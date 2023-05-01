using Models;

namespace Basket.API.Services.Interfaces
{
    public interface ICatalogService
    {
        Task<Concert> GetConcert(string concertId);
    }
}