using System.Diagnostics;
using Basket.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using QueueFactory.Models;
using QueueFactory.Models.Interfaces;
using RabbitMQ.Client;
using commonModels = Models;

namespace Basket.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly ICatalogService _catalogService;
        private readonly IBus _bus;

        //Important: The name of the Activity should be the same as the name of the Source added in the Web API startup AddOpenTelemetryTracing Builder
        private static readonly ActivitySource Activity = new("APITracing");
        private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

        public BasketController(ICatalogService catalogService, IBus bus)
        {
            _catalogService = catalogService;
            _bus = bus;
        }

        [HttpPost]
        public async Task<IActionResult> AddBasketItem([FromBody] commonModels.BasketItem item)
        {
            var concert = await _catalogService.GetConcert(item.ConcertId);
            return Ok(item);
        }

        [HttpPost]
        [Route("checkout")]
        public async Task<IActionResult> Checkout([FromBody] commonModels.Basket basket)
        {
            using (var activity = Activity.StartActivity("RabbitMq Publish", ActivityKind.Producer))
            {
                var basicProperties = _bus.GetBasicProperties();
                AddActivityToHeader(activity, basicProperties);

                await _bus.SendAsync(QueueType.Processing, new BasketRequest()
                {
                    Basket = basket
                }, basicProperties);
            }

            return Ok(basket);
        }

        private void AddActivityToHeader(Activity activity, IBasicProperties props)
        {
            try
            {
                Propagator.Inject(new PropagationContext(activity.Context, Baggage.Current), props, InjectContextIntoHeader);
                activity?.SetTag("messaging.system", "rabbitmq");
                activity?.SetTag("messaging.destination_kind", "queue");
                activity?.SetTag("messaging.rabbitmq.queue", "sample"); //TODO: Glenn - Queue name?
                activity?.SetTag("messaging.destination", string.Empty);
                activity?.SetTag("messaging.rabbitmq.routing_key", QueueType.Processing);
            }
            catch (Exception ex)
            {
                var t = ex.Message;
            }
        }

        private void InjectContextIntoHeader(IBasicProperties props, string key, string value)
        {
            try
            {
                props.Headers ??= new Dictionary<string, object>();
                props.Headers[key] = value;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Failed to inject trace context");
            }
        }
    }
}