using System;
using System.Net;
using System.Xml.Linq;
using Catalog.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private List<Product> _products = new List<Product>();

        public CatalogController()
        {
            _products.Add(new Product("prod001", "Modern chair")
            {
                ImageURL = "assets/img/product-img/product1.jpg",
                HoverImageURL = "assets/img/product-img/product2.jpg",
                ThumbnailImageURL = "assets/img/bg-img/cart1.jpg",
                Price = 180,
                Rating = 4
            });

            _products.Add(new Product("prod002", "Vintage desk")
            {
                ImageURL = "assets/img/product-img/product4.jpg",
                HoverImageURL = "assets/img/product-img/product3.jpg",
                ThumbnailImageURL = "assets/img/bg-img/cart2.jpg",
                Price = 250,
                Rating = 4
            });

            _products.Add(new Product("prod003", "Lounge chair")
            {
                ImageURL = "assets/img/product-img/product5.jpg",
                HoverImageURL = "assets/img/product-img/product6.jpg",
                ThumbnailImageURL = "assets/img/bg-img/cart3.jpg",
                Price = 300,
                Rating = 4
            });
        }

        [HttpGet]
        [Route("items")]
        public IActionResult Items()
        {
            return Ok(_products);
        }
    }
}