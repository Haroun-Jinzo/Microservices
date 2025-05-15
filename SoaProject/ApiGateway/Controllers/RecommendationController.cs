using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Soa.Protos;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RecommendationController : ControllerBase
    {
        private readonly UserService.UserServiceClient _userClient;
        private readonly ProductService.ProductServiceClient _productClient;
        private readonly IKafkaProducer _kafkaProducer;
        private readonly ILogger<RecommendationController> _logger;

        public RecommendationController(
            UserService.UserServiceClient userClient,
            ProductService.ProductServiceClient productClient,
            IKafkaProducer kafkaProducer,
            ILogger<RecommendationController> logger)
        {
            _userClient = userClient;
            _productClient = productClient;
            _kafkaProducer = kafkaProducer;
            _logger = logger;
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Product>>> GetRecommendations(string userId)
        {
            try
            {
                // 1. Get user preferences
                var userResponse = await _userClient.GetUserPreferencesAsync(
                    new UserRequest { UserId = userId },
                    deadline: DateTime.UtcNow.AddSeconds(5)
                );

                if (userResponse == null || userResponse.Preferences.Count == 0)
                    return NotFound("No user preferences found");

                // 2. Get products
                var category = userResponse.Preferences.FirstOrDefault() ?? "default";
                var productsResponse = await _productClient.GetProductsByCategoryAsync(
                    new CategoryRequest { Category = category },
                    deadline: DateTime.UtcNow.AddSeconds(5)
                );

                return Ok(productsResponse.Products.Take(5));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Recommendation error for user {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("interactions")]
        public async Task<IActionResult> LogInteraction(
            [FromBody] InteractionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _kafkaProducer.ProduceAsync(
                    "user-interactions",
                    JsonSerializer.Serialize(new {
                        request.UserId,
                        request.ProductId,
                        Timestamp = DateTime.UtcNow
                    })
                );
                
                return Ok(new { Success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kafka error for user {UserId}", request.UserId);
                return StatusCode(500, "Event logging failed");
            }
        }

        public record InteractionRequest(
            [Required] string UserId,
            [Required] string ProductId
        );

        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { status = "healthy" });
        }
    }
}