using System;
using Models;

namespace Basket.API.Services.Interfaces
{
    public interface ICatalogService
    {
        Task<Product> GetProduct(string productId);
    }
}