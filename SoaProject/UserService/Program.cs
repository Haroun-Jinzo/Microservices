using Microsoft.AspNetCore.Server.Kestrel.Core;
using MongoDB.Driver;
using UserService.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();

builder.WebHost.ConfigureKestrel(opt => {
    opt.ListenAnyIP(5001, listenOptions => {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});


var mongoClient = new MongoClient(builder.Configuration["MongoDB:ConnectionString"]);
var database = mongoClient.GetDatabase("recommendation_db");
builder.Services.AddSingleton(database);

var app = builder.Build();
app.MapGrpcService<UserService.Services.UserService>();
app.Run();