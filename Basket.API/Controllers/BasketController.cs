using Basket.API.Models;
using Basket.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly ICatalogService _catalogService;

        public BasketController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        [HttpPost]
        public async Task<IActionResult> AddBasketItem([FromBody] BasketItem item)
        {
            var concert = await _catalogService.GetConcert(item.ConcertId);
            return Ok(item);
        }

        [HttpPost]
        [Route("checkout")]
        public IActionResult Checkout([FromBody] Basket.API.Models.Basket basket)
        {
            return Ok(basket);
        }
    }
}