using System.Security.Cryptography.X509Certificates;
using Soa.Protos; // Ensure the namespace for ProductService is included
using Grpc.Net.Client; // Add gRPC client namespace
using Google.Protobuf; // Add Protobuf namespace
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using HotChocolate.Execution.Configuration;
using Grpc.Core;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration["Jwt:Key"] 
    ?? throw new InvalidOperationException("JWT Key not configured");
var userServiceUri = builder.Configuration["GrpcServices:UserService"] 
    ?? throw new InvalidOperationException("UserService URL not configured");
var productServiceUri = builder.Configuration["GrpcServices:ProductService"] 
    ?? throw new InvalidOperationException("ProductService URL not configured");
var kafkaBootstrap = builder.Configuration["Kafka:BootstrapServers"] 
    ?? throw new InvalidOperationException("Kafka config missing");

// 1. Add Controllers and Routing
builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => 
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    }); 

builder.Services.AddAuthorization();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddErrorFilter(error => 
        error.Exception != null 
            ? error.WithMessage(error.Exception.Message + "swwwwwwwwwwwwwwwwwwwwwww") 
            : error.WithMessage("Internal server error"))
    .ModifyRequestOptions(opt => 
    {
        opt.IncludeExceptionDetails = builder.Environment.IsDevelopment();
    });


// 2. Configure gRPC Clients
builder.Services.AddGrpcClient<UserService.UserServiceClient>(o => 
    o.Address = new Uri(builder.Configuration["GrpcServices:UserService"]!))
.ConfigureChannel(options => {
    options.Credentials = ChannelCredentials.Insecure;
});

// Repeat for ProductService
builder.Services.AddGrpcClient<ProductService.ProductServiceClient>(options => 
    options.Address = new Uri(builder.Configuration["GrpcServices:ProductService"]!))
.ConfigureChannel(options => {
    options.Credentials = ChannelCredentials.Insecure;
});

// 3. Configure Kafka
builder.Services.AddSingleton<IKafkaProducer>(sp => 
    new KafkaProducer(
        builder.Configuration["Kafka:BootstrapServers"]!, 
        sp.GetRequiredService<ILogger<KafkaProducer>>()));

// 4. Build Middleware Pipeline
var app = builder.Build();

// Correct middleware order
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization(); // ← Must come after UseRouting()

app.MapControllers();
app.MapGraphQL("/graphql");

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapGet("/debug/routes", async context =>
    {
        var endpointDataSource = context.RequestServices.GetRequiredService<EndpointDataSource>();
        var sb = new StringBuilder();
        sb.AppendLine("Registered Endpoints:");
        foreach (var endpoint in endpointDataSource.Endpoints)
        {
            sb.AppendLine($"- {endpoint.DisplayName}");
        }
        await context.Response.WriteAsync(sb.ToString());
    });
});

app.Run();