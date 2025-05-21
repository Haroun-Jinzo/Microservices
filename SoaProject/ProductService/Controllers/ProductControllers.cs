using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace ProductService.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {

        private readonly IMongoCollection<Product>? _products;

        public ProductController(IMongoDatabase db)
        {
        _products = db.GetCollection<Product>("products");
        }
    
        [HttpGet]
        public IActionResult GetAllProducts()
        {
            return Ok(new[] 
            {
                new { Id = 1, Name = "Sample Product" }
            });
        }

        [HttpPost]
        //[Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newProduct = new Product
            {
                Name = request.Name,
                Category = request.Category,
                Description = request.Description,
                Price = request.Price
            };

            await _products.InsertOneAsync(newProduct);

            return CreatedAtAction(
                nameof(GetAllProducts), 
                new { id = newProduct.Id }, 
                newProduct);
        }

        public record CreateProductDto(
            [Required] string Name,
            [Required] string Category,
            string Description,
            [Range(0, double.MaxValue)] double Price);

    }
}