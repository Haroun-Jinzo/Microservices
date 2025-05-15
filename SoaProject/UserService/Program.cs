using Microsoft.AspNetCore.Server.Kestrel.Core;
using MongoDB.Driver;
using UserService.Services; // Ensure this namespace contains the UserService class

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();

builder.WebHost.ConfigureKestrel(opt => {
    opt.ListenAnyIP(5001, listenOptions => {
        listenOptions.Protocols = HttpProtocols.Http2; // Force HTTP/2
    });
});

// MongoDB Configuration
var mongoClient = new MongoClient(builder.Configuration["MongoDB:ConnectionString"]);
var database = mongoClient.GetDatabase("recommendation_db");
builder.Services.AddSingleton(database);

var app = builder.Build();
app.MapGrpcService<UserService.Services.UserService>(); // Fully qualify the UserService class
app.Run();