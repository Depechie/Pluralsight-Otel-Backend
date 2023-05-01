using Microsoft.AspNetCore.Mvc;
using Models;

namespace Catalog.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private List<Concert> _concerts = new List<Concert>();

        public CatalogController()
        {
            _concerts.Add(new Concert("conc001", "Alexander Lemtov Live", "Alexander Lemtov Live", "Alexander Lemtov")
            {
                ImageURL = "assets/img/product-img/concert_001.jpg",
                HoverImageURL = "assets/img/product-img/concert_002.jpg",
                ThumbnailImageURL = "assets/img/bg-img/cart1.jpg",
                DateFrom = DateTime.Now.AddMonths(2),
                DateTo = DateTime.Now.AddMonths(2).AddHours(4),
                Price = 65.00M,
            });

            _concerts.Add(new Concert("conc002", "To The Moon And Back", "To The Moon And Back", "Santiago Martinez")
            {
                ImageURL = "assets/img/product-img/concert_003.jpg",
                HoverImageURL = "assets/img/product-img/concert_004.jpg",
                ThumbnailImageURL = "assets/img/bg-img/cart2.jpg",
                DateFrom = DateTime.Now.AddMonths(2),
                DateTo = DateTime.Now.AddMonths(2).AddHours(4),
                Price = 135.00M,
            });

            _concerts.Add(new Concert("conc003", "The State Of Affairs: Mariam Live!", "The State Of Affairs: Mariam Live!", "	Mariam Johnson")
            {
                ImageURL = "assets/img/product-img/concert_005.jpg",
                HoverImageURL = "assets/img/product-img/concert_006.jpg",
                ThumbnailImageURL = "assets/img/bg-img/cart3.jpg",
                DateFrom = DateTime.Now.AddMonths(2),
                DateTo = DateTime.Now.AddMonths(2).AddHours(4),
                Price = 85.00M,
            });
        }

        [HttpGet]
        [Route("items")]
        public IActionResult Items()
        {
            return Ok(_concerts);
        }

        [HttpGet]
        [Route("items/{id}")]
        public IActionResult GetItem(string id)
        {
            return Ok(_concerts.FirstOrDefault(i => string.Equals(i.Id, id, StringComparison.InvariantCultureIgnoreCase)));
        }
    }
}