using CcStore.Models;
using MongoDB.Driver;

namespace CcStore.Repository.Contracts
{
    public interface IProductRepository : IRepositoryBase<Product>
    {
        Task<long> CountDocumentsAsync(FilterDefinition<Product> filter);
        Task<Review> PostReviewAsync(string productId, Review review, string username);
    }
}