using AppCaching.Data;
using AppCaching.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Entity Framework Core with SQLite
builder.Services.AddDbContext<AppDbContext>(o =>
{
    o.UseSqlite(builder.Configuration["ConnectionStrings:SqlLite"]);
});

builder.Services.AddSingleton<AppDbSeed>();

// Configure Memory Cache and Redis Cache
#region CacheSettings
// This section configures the caching services for the application.
// It sets up both InMemory and Redis caching based on the configuration settings.
builder.Services.AddMemoryCache();
builder.Services.AddStackExchangeRedisCache(o =>
{
    o.Configuration = builder.Configuration["CacheSettings:RedisConnectionString"] ?? "localhost:6379";
});
#endregion

builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

//Seed Data to the Database
((IApplicationBuilder)app).ApplicationServices.GetRequiredService<AppDbSeed>();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
