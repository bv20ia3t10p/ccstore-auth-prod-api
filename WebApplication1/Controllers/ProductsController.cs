using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CcStore.Models;
using CcStore.Service;
using CcStore.Repository;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;
    private readonly UserRepository _userRepository;

    public ProductsController(ProductService productService, UserRepository userRepository)
    {
        _productService = productService;
        _userRepository = userRepository;
    }

    // Public endpoint with pagination
    [HttpGet]
    public async Task<IActionResult> GetAllProducts([FromQuery] int skip = 0, [FromQuery] int limit = 30)
    {
        var (products, total) = await _productService.GetAllProductsAsync(skip, limit);

        var result = new
        {
            products = products.Select(p => new
            {
                p.Id,
                p.Title,
                p.Description,
                p.Category,
                p.Price,
                p.DiscountPercentage,
                p.Rating,
                p.Stock,
                p.Tags,
                p.Brand,
                p.Sku,
                p.Weight,
                p.Dimensions,
                p.WarrantyInformation,
                p.ShippingInformation,
                p.AvailabilityStatus,
                Reviews = p.Reviews.Select(r => new
                {
                    r.Rating,
                    r.Comment,
                    r.Date,
                    r.ReviewerName,
                    r.ReviewerEmail,
                    r.Images
                }).ToList(),
                p.ReturnPolicy,
                p.MinimumOrderQuantity,
                Meta = new
                {
                    p.Meta.CreatedAt,
                    p.Meta.UpdatedAt,
                    p.Meta.Barcode,
                    p.Meta.QrCode
                },
                p.Images,
                p.Thumbnail
            }).ToList(),
            total,
            skip,
            limit
        };

        return Ok(result);
    }

    // Protected endpoint requiring JWT authentication
    [Authorize]
    [HttpPost("{productId}/reviews")]
    public async Task<IActionResult> PostReview(string productId, [FromBody] Review review)
    {
        var username = User.Identity.Name; // Get the username from the JWT token
        var user = await _userRepository.GetUserByUsernameOrEmailAsync(username);
        Review reviewToPost = new() {
            ReviewerName = user.FirstName  + " " + user.LastName,
            ReviewerEmail = user.Email,
            Comment = review.Comment,
            Rating = review.Rating,
            Images = review.Images,
        };
        return Ok(await _productService.PostReviewAsync(productId, reviewToPost, username));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(string id)
    {
        var product = await _productService.GetProductByIdAsync(id);

        if (product == null)
        {
            return NotFound(new { message = "Product not found." });
        }

        var result = new
        {
            product.Id,
            product.Title,
            product.Description,
            product.Category,
            product.Price,
            product.DiscountPercentage,
            product.Rating,
            product.Stock,
            product.Tags,
            product.Brand,
            product.Sku,
            product.Weight,
            product.Dimensions,
            product.WarrantyInformation,
            product.ShippingInformation,
            product.AvailabilityStatus,
            Reviews = product.Reviews.Select(r => new
            {
                r.Rating,
                r.Comment,
                r.Date,
                r.ReviewerName,
                r.ReviewerEmail,
                r.Images
            }).ToList(),
            product.ReturnPolicy,
            product.MinimumOrderQuantity,
            Meta = new
            {
                product.Meta.CreatedAt,
                product.Meta.UpdatedAt,
                product.Meta.Barcode,
                product.Meta.QrCode
            },
            product.Images,
            product.Thumbnail
        };

        return Ok(result);
    }

}
