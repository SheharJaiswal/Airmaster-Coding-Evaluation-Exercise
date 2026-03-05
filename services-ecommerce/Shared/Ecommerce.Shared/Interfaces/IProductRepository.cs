using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.Entities;

namespace Ecommerce.Shared.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(string id);
        Task<List<Product>> GetAllAsync();
        Task<List<Product>> GetByCategoryAsync(string category);
        Task<Product> CreateAsync(ProductCreateDto productDto);
        Task<Product?> UpdateAsync(string id, ProductUpdateDto productDto);
        Task<bool> DeleteAsync(string id);
    }
}
