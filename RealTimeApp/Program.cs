using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RealTimeApp;
using RealTimeApp.Hubs;
using RealTimeApp.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TestdbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddHostedService<PostgresListener>();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapHub<YourHub>("/yourhub");

app.Run();
