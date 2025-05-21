using Microsoft.AspNetCore.Mvc;
using Soa.Protos;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Grpc.Core;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly UserService.UserServiceClient _userClient;
        private readonly ILogger<UserController> _logger;

        public UserController(
            UserService.UserServiceClient userClient,
            ILogger<UserController> logger)
        {
            _userClient = userClient;
            _logger = logger;
        }

        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto request)
        {
            try
            {
                var grpcRequest = new CreateUserRequest
                {
                    UserId = request.UserId,
                    Preferences = { request.Preferences ?? new List<string>() }
                };

                var response = await _userClient.CreateUserAsync(grpcRequest);

                return CreatedAtAction(
                    nameof(GetUserPreferences), 
                    new { userId = response.UserId }, 
                    new UserResponseDto(
                        response.UserId,
                        response.Preferences.ToList()
                    ));
            }
            catch (RpcException ex) when (ex is not RpcException)
            {
                _logger.LogWarning("User {UserId} already exists", request.UserId);
                return Conflict($"User {request.UserId} already exists");
            }
            catch (Exception ex) when (ex is not RpcException)
            {
                _logger.LogError(ex, "Unexpected error creating user {UserId}", request.UserId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{userId}/preferences")]
        public async Task<IActionResult> GetUserPreferences(string userId)
        {
            try
            {
                var response = await _userClient.GetUserPreferencesAsync(
                    new UserRequest { UserId = userId });

                return Ok(new UserPreferencesDto(
                    response.UserId,
                    response.Preferences.ToList()
                ));
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error fetching preferences for user {UserId}", userId);
                return StatusCode(500, "Service unavailable");
            }
        }


        public record CreateUserDto(
            [Required] string UserId,
            [Required] List<string> Preferences);

        public record UserResponseDto(
            string UserId,
            List<string> Preferences);

        public record UserPreferencesDto(
            string UserId,
            List<string> Preferences);
    }
}