using Ecommerce.Shared.Database;
using Microsoft.OpenApi.Models;
using ProductService.Startup;
using ProductService.WebApi.Services;
using Steeltoe.Discovery.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddDbConnection(builder.Configuration);
builder.Services.AddRequiredServices();

// Redis Caching
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
    options.InstanceName = "ProductCache:";
});
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Product Service API", Version = "v1" });
});
builder.Services.AddDiscoveryClient(builder.Configuration);

var app = builder.Build();

// Centralized Database Migration - handles all services in correct order
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    MigrationInitializer.InitializeDatabase(scope.ServiceProvider, logger);
}

// Configure pipeline
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseDiscoveryClient(); // Register with Eureka

app.Run();