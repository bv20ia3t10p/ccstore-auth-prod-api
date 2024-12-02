using CcStore.Models;
using CcStore.Repository.Contracts;
using MongoDB.Driver;

namespace CcStore.Repository
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        private readonly IMongoCollection<Product> _productCollection;

        public ProductRepository(IMongoDatabase database)
            : base(database, "Products")  // Passing the collection name to the base class
        {
            _productCollection = database.GetCollection<Product>("Products");
        }

        // You can add custom methods specific to the Product entity here, e.g.:
        public async Task<Review> PostReviewAsync(string productId, Review review, string username)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Id, Int32.Parse(productId));
            var update = Builders<Product>.Update.Push(p => p.Reviews, review);
            await _productCollection.UpdateOneAsync(filter, update);
            return review;
        }
        // Custom method to count documents in the Products collection
        public async Task<long> CountDocumentsAsync(FilterDefinition<Product> filter)
        {
            return await _productCollection.CountDocumentsAsync(filter);
        }
    }
}
