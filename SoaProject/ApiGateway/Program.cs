using System.Security.Cryptography.X509Certificates;
using Soa.Protos;
using Grpc.Net.Client; 
using Google.Protobuf; 
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using HotChocolate.Execution.Configuration;
using Grpc.Core;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddAuthorization(options => 
{
    options.AddPolicy("AdminOnly", policy => 
        policy.RequireRole("Administrator"));
});

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



builder.Services.AddGrpcClient<UserService.UserServiceClient>(o => 
    o.Address = new Uri(builder.Configuration["GrpcServices:UserService"]!))
.ConfigureChannel(options => {
    options.Credentials = ChannelCredentials.Insecure;
});


builder.Services.AddGrpcClient<ProductService.ProductServiceClient>(options => 
    options.Address = new Uri(builder.Configuration["GrpcServices:ProductService"]!))
.ConfigureChannel(options => {
    options.Credentials = ChannelCredentials.Insecure;
});


builder.Services.AddSingleton<IKafkaProducer>(sp => 
    new KafkaProducer(
        builder.Configuration["Kafka:BootstrapServers"]!, 
        sp.GetRequiredService<ILogger<KafkaProducer>>()));


var app = builder.Build();


app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

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