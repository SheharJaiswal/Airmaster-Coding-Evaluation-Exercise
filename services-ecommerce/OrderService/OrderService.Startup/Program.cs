using Ecommerce.Shared.Database;
using Microsoft.OpenApi.Models;
using Steeltoe.Discovery.Client;
using OrderService.Startup;
using OrderService.Startup.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddDbConnection(builder.Configuration);
builder.Services.AddRequiredServices();

// SignalR for real-time updates
builder.Services.AddSignalR();
builder.Services.AddSingleton<IOrderNotificationService, OrderNotificationService>();

//builder.Services.AddRabbitMqConnection(builder.Configuration);
// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Order Service API", Version = "v1" });
});
builder.Services.AddDiscoveryClient(builder.Configuration);

// CORS for SignalR
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

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
app.UseCors("AllowAll");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<OrderStatusHub>("/hubs/orderstatus");
app.UseDiscoveryClient(); // Register with Eureka

app.Run();