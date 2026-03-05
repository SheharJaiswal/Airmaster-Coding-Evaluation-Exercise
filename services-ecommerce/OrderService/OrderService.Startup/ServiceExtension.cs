using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Ecommerce.Shared.Interfaces;
using Ecommerce.Shared.Repositories;
using Ecommerce.Shared.Services;
using OrderService.WebApi.Data;
using RabbitMQ.Client;
using System.Text;

namespace OrderService.Startup
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                };
            });
            return services;
        }
        public static IServiceCollection AddRequiredServices(this IServiceCollection services)
        {
            //services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            
            // Register Shipping Service (Mock for prototype, can be swapped with real FedEx/UPS API)
            services.AddScoped<IShippingService, MockShippingService>();
            
            return services;
        }
        public static IServiceCollection AddDbConnection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<OrderDbContext>(options =>
                options.UseMySql(
                    configuration.GetConnectionString("MySql"),
                    new MySqlServerVersion(new Version(8, 0, 33)),
                    mySqlOptions =>
                    {
                        mySqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    }
                )
            );
            services.AddScoped<Ecommerce.Shared.Interfaces.DbContexts.IOrderDbContext>(provider =>
                provider.GetRequiredService<OrderDbContext>());
            return services;
        }
        public static IServiceCollection AddRabbitMqConnection(this IServiceCollection services, IConfiguration configuration)
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
                UserName = configuration["RabbitMQ:UserName"] ?? "guest",
                Password = configuration["RabbitMQ:Password"] ?? "guest"
            };

            // Create and register singleton IConnection
            var connection = factory.CreateConnection("OrderService");
            services.AddSingleton<IConnection>(connection);

            return services;
        }
    }
}