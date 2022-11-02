using System;
using System.Text.Json;
using Basket.API.Services.Interfaces;
using Models;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace Basket.API.Services
{
    public class CatalogService : ICatalogService
    {
        private HttpClient _httpClient;
        private JsonSerializerOptions _options;

        public CatalogService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<Product> GetProduct(string productId)
        {
            return JsonSerializer.Deserialize<Product>(await _httpClient.GetStringAsync($"https://localhost:7001/api/v1/catalog/items/{productId}"), _options);
        }
    }
}