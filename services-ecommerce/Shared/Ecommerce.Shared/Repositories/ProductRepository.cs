using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Interfaces.DbContexts;
using Ecommerce.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Shared.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IProductDbContext _context;

        public ProductRepository(IProductDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdAsync(string id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<List<Product>> GetByCategoryAsync(string category)
        {
            return await _context.Products
                .Where(p => p.Category == category)
                .ToListAsync();
        }

        public async Task<Product> CreateAsync(ProductCreateDto productDto)
        {
            var now = DateTime.UtcNow;
            var product = new Product
            {
                Id = Guid.NewGuid().ToString(),
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Category = productDto.Category,
                StockQuantity = productDto.StockQuantity,
                CreatedAt = now,
                CreatedBy = "System",
                UpdatedAt = now,
                UpdatedBy = "System"
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> UpdateAsync(string id, ProductUpdateDto productDto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return null;

            if (productDto.Description != null)
                product.Description = productDto.Description;
            if (productDto.Price.HasValue)
                product.Price = productDto.Price.Value;
            if (productDto.StockQuantity.HasValue)
                product.StockQuantity = productDto.StockQuantity.Value;

            product.UpdatedAt = DateTime.UtcNow;
            product.UpdatedBy = "System";
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
