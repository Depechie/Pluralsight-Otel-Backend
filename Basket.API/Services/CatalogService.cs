using System.Text.Json;
using Basket.API.Services.Interfaces;
using Models;

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

        public async Task<Concert> GetConcert(string concertId)
        {
            return JsonSerializer.Deserialize<Concert>(await _httpClient.GetStringAsync($"http://localhost:6001/api/v1/catalog/items/{concertId}"), _options);
        }
    }
}