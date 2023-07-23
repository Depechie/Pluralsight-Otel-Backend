using System;
using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using QueueFactory;
using QueueFactory.Models;
using QueueFactory.Models.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ServiceWorker.Services.Interfaces;

namespace ServiceWorker
{
	public class Worker : BackgroundService
    {
        private readonly IBus _bus;
        private readonly ICatalogService _catalogService;

        //Important: The name of the Activity should be the same as the name of the Source added in the Web API startup AddOpenTelemetryTracing Builder
        private static readonly ActivitySource Activity = new("APITracing");
        private static readonly TextMapPropagator Propagator = new TraceContextPropagator();

        public Worker(ICatalogService catalogService)
        {
            _bus = RabbitMQFactory.CreateBus(BusType.LocalHost);
            _catalogService = catalogService;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await _bus.ReceiveAsync<BasketRequest>(QueueType.Processing, (message, args) =>
            {
                Task.Run(() => { ProcessMessage(message, args); }, cancellationToken);
            });
        }

        private async Task ProcessMessage(BasketRequest message, BasicDeliverEventArgs args)
        {
            var parentContext = Propagator.Extract(default, args.BasicProperties, ExtractTraceContextFromBasicProperties);
            Baggage.Current = parentContext.Baggage;

            using (var activity = Activity.StartActivity("Process Message", ActivityKind.Consumer, parentContext.ActivityContext))
            {
                AddActivityTags(activity);

                List<Task> tasks = new List<Task>();
                foreach(var concertId in message?.Basket?.ConcertIds)
                {
                    tasks.Add(Task.Run(() => _catalogService.GetConcert(concertId)));
                }

                await Task.WhenAll(tasks);
            }
        }

        private IEnumerable<string> ExtractTraceContextFromBasicProperties(IBasicProperties props, string key)
        {
            try
            {
                if (props.Headers.TryGetValue(key, out var value))
                {
                    var bytes = value as byte[];
                    return new[] { Encoding.UTF8.GetString(bytes) };
                }
            }
            catch (Exception ex)
            {
            }

            return Enumerable.Empty<string>();
        }

        private void AddActivityTags(Activity activity)
        {
            activity?.SetTag("messaging.system", "rabbitmq");
            activity?.SetTag("messaging.destination_kind", "queue");
            activity?.SetTag("messaging.rabbitmq.queue", "sample"); //TODO: Glenn - Queue name?
            activity?.SetTag("messaging.destination", string.Empty);
            activity?.SetTag("messaging.rabbitmq.routing_key", QueueType.Processing);
        }
    }
}

