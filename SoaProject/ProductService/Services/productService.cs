// ProductService/Services/ProductService.cs
using Grpc.Core;
using MongoDB.Driver;
using Soa.Protos;

namespace ProductService.Services
{
    public class ProductService : Soa.Protos.ProductService.ProductServiceBase
    {
        private readonly IMongoCollection<Product> _products;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IMongoDatabase database, ILogger<ProductService> logger)
        {
            _products = database.GetCollection<Product>("products");
            _logger = logger;
        }

        public override async Task<ProductsResponse> GetProductsByCategory(CategoryRequest request, ServerCallContext context)
        {
            try
            {
                var filter = request.Category.ToLower() == "default" 
                    ? Builders<Product>.Filter.Empty 
                    : Builders<Product>.Filter.Eq(p => p.Category, request.Category);

                var products = await _products
                    .Find(filter)
                    .Limit(10)
                    .ToListAsync();

                var response = new ProductsResponse();
                response.Products.AddRange(products.Select(p => new Soa.Protos.Product
                {
                    Id = p.Id ?? "",
                    Name = p.Name,
                    Category = p.Category,
                }));

                _logger.LogInformation("Retrieved {Count} products for category {Category}", 
                    response.Products.Count, request.Category);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products for category {Category}", request.Category);
                throw new RpcException(new Status(
                    StatusCode.Internal,
                    "Failed to retrieve products"));
            }
        }

        public override async Task<ProductsResponse> CreateProduct(ProductRequest request, ServerCallContext context)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Product name is required"));

                var existingProduct = await _products.Find(p => p.Name == request.Name).FirstOrDefaultAsync();

                if(existingProduct != null)
                    throw new RpcException(new Status(StatusCode.AlreadyExists, "Product already exists"));

                var newProduct = new Product
                {
                    Name = request.Name,
                    Category = request.Category,
                    Description = request.Category,
                    Price = request.Price
                };

                await _products.InsertOneAsync(newProduct);

                _logger.LogInformation($"Created product {newProduct.Id}");


                var response = new ProductsResponse();
                response.Products.Add(new Soa.Protos.Product
                {
                    Id = newProduct.Id,
                    Name = newProduct.Name,
                    Category = newProduct.Category
                });

                return response;
        }
    }
}
