using System;
using Basket.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        [HttpPost]
        public IActionResult AddBasketItem([FromBody] BasketItem data)
        {
            return Ok();
        }
    }
}