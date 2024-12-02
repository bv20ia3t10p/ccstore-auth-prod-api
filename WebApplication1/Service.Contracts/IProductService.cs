using CcStore.Models;

namespace CcStore.Service.Contracts
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Review> PostReviewAsync(string productId, Review review, string username);
    }
}