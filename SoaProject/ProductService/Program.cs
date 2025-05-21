using Microsoft.AspNetCore.Server.Kestrel.Core;
using MongoDB.Driver;
using ProductService.Services; // Ensure this namespace contains the correct ProductService class  

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();
builder.Services.AddControllers();

var mongoClient = new MongoClient(builder.Configuration["MongoDB:ConnectionString"]);
var database = mongoClient.GetDatabase("recommendation_db");
builder.Services.AddSingleton(database);

builder.WebHost.ConfigureKestrel(opt => {
    opt.ListenAnyIP(5002, listenOptions => {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

var app = builder.Build();

var products = database.GetCollection<Product>("products");
products.Indexes.CreateOne(
    new CreateIndexModel<Product>(
        Builders<Product>.IndexKeys
            .Ascending(p => p.Name)
            .Ascending(p => p.Category)
    )
);

app.MapGrpcService<ProductService.Services.ProductService>(); // Fully qualify the ProductService class to avoid ambiguity 
app.MapControllers(); 
app.Run();