using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ProductService.Startup.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepo;

        public ProductsController(IProductRepository productRepo)
        {
            _productRepo = productRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _productRepo.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            return product != null ? Ok(product) : NotFound();
        }

        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetByCategory(string category)
        {
            return Ok(await _productRepo.GetByCategoryAsync(category));
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateDto dto)
        {
            var product = await _productRepo.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, ProductUpdateDto dto)
        {
            var product = await _productRepo.UpdateAsync(id, dto);
            return product != null ? Ok(product) : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            return await _productRepo.DeleteAsync(id) ? NoContent() : NotFound();
        }
    }
}