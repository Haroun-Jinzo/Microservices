using Grpc.Core;
using MongoDB.Driver;
using Soa.Protos;

namespace UserService.Services
{
    public class UserService : Soa.Protos.UserService.UserServiceBase
    {
        private readonly IMongoCollection<User> _users;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IMongoDatabase database, 
            ILogger<UserService> logger)
        {
            _users = database.GetCollection<User>("users");
            _logger = logger;
        }

        public override async Task<UserResponse> GetUserPreferences(
            UserRequest request, 
            ServerCallContext context)
        {
            try
            {
                var user = await _users.Find(u => u.UserId == request.UserId)
                                      .FirstOrDefaultAsync();

                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found", request.UserId);
                    throw new RpcException(new Status(
                        StatusCode.NotFound, 
                        $"User {request.UserId} not found"));
                }

                return new UserResponse
                {
                    UserId = user.UserId,
                    Preferences = { user.Preferences }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting preferences for user {UserId}", request.UserId);
                throw new RpcException(new Status(
                    StatusCode.Internal, 
                    "Internal server error"));
            }
        }

        public override async Task<UserResponse> CreateUser(
            CreateUserRequest request, 
            ServerCallContext context)
        {
            try
            {
                // Check for existing user
                var existingUser = await _users.Find(u => u.UserId == request.UserId)
                                             .FirstOrDefaultAsync();
                
                if (existingUser != null)
                {
                    _logger.LogWarning("User {UserId} already exists", request.UserId);
                    throw new RpcException(new Status(
                        StatusCode.AlreadyExists, 
                        $"User {request.UserId} already exists"));
                }

                // Create new user
                var newUser = new User
                {
                    UserId = request.UserId,
                    Preferences = request.Preferences.ToList()
                };

                await _users.InsertOneAsync(newUser);

                _logger.LogInformation("Created user {UserId}", request.UserId);
                
                return new UserResponse
                {
                    UserId = newUser.UserId,
                    Preferences = { newUser.Preferences }
                };
            }
            catch (MongoWriteException ex)
            {
                _logger.LogWarning("Duplicate key error for user {UserId}", request.UserId);
                throw new RpcException(new Status(
                    StatusCode.AlreadyExists, 
                    $"User {request.UserId} already exists"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user {UserId}", request.UserId);
                throw new RpcException(new Status(
                    StatusCode.Internal, 
                    "Failed to create user"));
            }
        }
    }
}