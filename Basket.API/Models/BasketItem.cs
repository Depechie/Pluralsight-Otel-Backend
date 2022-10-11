using System;
namespace Basket.API.Models
{
    public class BasketItem
    {
        public string ProductId { get; set; }
        public string BasketId { get; set; }
        public int Quantity { get; set; }

        public BasketItem(string productId, string basketId, int quantity)
        {
            ProductId = productId;
            BasketId = basketId;
            Quantity = quantity;
        }
    }
}

