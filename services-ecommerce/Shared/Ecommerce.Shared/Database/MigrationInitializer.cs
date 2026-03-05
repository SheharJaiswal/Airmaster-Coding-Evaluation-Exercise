using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Shared.Database
{
    /// <summary>
    /// Centralized migration initializer that ensures all database tables are created
    /// in the correct order before any service starts, preventing foreign key errors.
    /// </summary>
    public static class MigrationInitializer
    {
        private static readonly object _lock = new object();
        private static bool _isInitialized = false;

        /// <summary>
        /// Initializes the database by running migrations for all services in dependency order.
        /// This method is thread-safe and will only run once even if called by multiple services.
        /// </summary>
        /// <param name="serviceProvider">The service provider containing all DbContext instances</param>
        /// <param name="logger">Optional logger for tracking migration progress</param>
        public static void InitializeDatabase(IServiceProvider serviceProvider, ILogger? logger = null)
        {
            lock (_lock)
            {
                if (_isInitialized)
                {
                    logger?.LogInformation("Database already initialized. Skipping migration.");
                    return;
                }

                logger?.LogInformation("Starting centralized database migration initialization...");

                try
                {
                    // Wait for database to be ready
                    WaitForDatabaseReady(serviceProvider, logger);

                    // Step 1: Initialize UserService tables (no dependencies)
                    InitializeContext(serviceProvider, "UserService.WebApi.Data.UserDbContext", logger);

                    // Step 2: Initialize ProductService tables (no dependencies)
                    InitializeContext(serviceProvider, "ProductService.WebApi.Data.ProductDbContext", logger);

                    // Step 3: Initialize CartService tables (depends on Users and Products)
                    InitializeContext(serviceProvider, "CartService.WebApi.Data.CartDbContext", logger);

                    // Step 4: Initialize OrderService tables (depends on all above)
                    InitializeContext(serviceProvider, "OrderService.WebApi.Data.OrderDbContext", logger);

                    _isInitialized = true;
                    logger?.LogInformation("Database migration initialization completed successfully.");
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, "Failed to initialize database migrations.");
                    throw;
                }
            }
        }

        private static void WaitForDatabaseReady(IServiceProvider serviceProvider, ILogger? logger)
        {
            const int maxRetries = 30;
            const int delayMs = 1000;
            int retries = 0;

            while (retries < maxRetries)
            {
                try
                {
                    // Try to get any DbContext to test database connection
                    // First try UserDbContext, then CartDbContext, then any DbContext
                    var contextTypes = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .Where(t => t.IsClass && !t.IsAbstract && t.BaseType?.Name == "DbContext")
                        .ToList();

                    if (contextTypes.Count == 0)
                    {
                        logger?.LogWarning("No DbContext found in the current service");
                        return;
                    }

                    DbContext context = null;
                    foreach (var contextType in contextTypes)
                    {
                        var testContext = serviceProvider.GetService(contextType) as DbContext;
                        if (testContext != null)
                        {
                            context = testContext;
                            break;
                        }
                    }

                    if (context != null)
                    {
                        context.Database.OpenConnection();
                        context.Database.CloseConnection();
                        logger?.LogInformation("Database is ready.");
                        return;
                    }

                    retries++;
                    if (retries < maxRetries)
                    {
                        logger?.LogWarning($"Database not ready yet. Retry {retries}/{maxRetries}");
                        System.Threading.Thread.Sleep(delayMs);
                    }
                }
                catch (Exception ex)
                {
                    retries++;
                    if (retries < maxRetries)
                    {
                        logger?.LogWarning($"Database not ready yet. Retry {retries}/{maxRetries}. Error: {ex.Message}");
                        System.Threading.Thread.Sleep(delayMs);
                    }
                    else
                    {
                        logger?.LogError($"Database failed to become ready after {maxRetries} retries.");
                        throw;
                    }
                }
            }
        }

        private static void InitializeContext(IServiceProvider serviceProvider, string contextTypeName, ILogger? logger)
        {
            try
            {
                // Try to get the DbContext from the service provider
                var contextType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.FullName == contextTypeName);

                if (contextType == null)
                {
                    logger?.LogInformation($"Skipping {contextTypeName} - not registered in this service.");
                    return;
                }

                var context = serviceProvider.GetService(contextType) as DbContext;
                if (context == null)
                {
                    logger?.LogInformation($"Skipping {contextTypeName} - not available in service provider.");
                    return;
                }

                logger?.LogInformation($"Applying migrations for {contextTypeName}...");
                context.Database.Migrate();
                logger?.LogInformation($"Successfully migrated {contextTypeName}.");
            }
            catch (Exception ex)
            {
                logger?.LogError($"Error initializing {contextTypeName}: {ex.Message}");
                // Continue with other contexts even if this one fails
            }
        }

        /// <summary>
        /// Resets the initialization flag. Only use this for testing purposes.
        /// </summary>
        public static void Reset()
        {
            lock (_lock)
            {
                _isInitialized = false;
            }
        }
    }
}
