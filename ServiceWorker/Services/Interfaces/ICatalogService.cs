using Models;

namespace ServiceWorker.Services.Interfaces
{
    public interface ICatalogService
    {
        Task<Concert> GetConcert(string concertId);
    }
}