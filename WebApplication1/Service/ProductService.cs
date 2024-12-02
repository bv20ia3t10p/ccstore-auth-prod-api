using CcStore.Models;
using CcStore.Repository;
using CcStore.Repository.Contracts;
using CcStore.Service.Contracts;
using MongoDB.Driver;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepo;
    private readonly IUserRepository _userRepo;

    public ProductService(ProductRepository productRepo, IUserRepository userRepo)
    {
        _productRepo = productRepo;
        _userRepo = userRepo;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await _productRepo.FindAllAsync();
    }

    public async Task<Review> PostReviewAsync(string productId, Review review, string username)
    {
        var user = await _userRepo.FindByConditionAsync(u => u.Username == username);
        var reviewer = user.FirstOrDefault();
        if (reviewer == null)
            throw new UnauthorizedAccessException("Invalid user");

        review.ReviewerName = reviewer.FirstName + " " + reviewer.LastName;
        return await _productRepo.PostReviewAsync(productId, review, username);
    }

    public async Task<(List<Product> products, long total)> GetAllProductsAsync(int skip, int limit)
    {
        var totalProducts = await _productRepo.CountDocumentsAsync(FilterDefinition<Product>.Empty);
        var products = (await _productRepo
            .FindAllAsync())
            .Skip(skip)
            .Take(limit)
            .ToList();
        return (products, totalProducts);
    }

    public async Task<Product> GetProductByIdAsync(string id)
    {
        var product = await _productRepo.FindByConditionAsync(p => p.Id.ToString() == id);
        return product.FirstOrDefault();
    }
}
