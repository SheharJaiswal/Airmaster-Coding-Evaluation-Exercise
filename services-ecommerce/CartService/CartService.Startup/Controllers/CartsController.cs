// CartService.Api/Controllers/CartsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ecommerce.Shared.Interfaces;
using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.Entities;

namespace CartService.Startup.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CartsController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;

        public CartsController(
            ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<CartResponseDto>> GetCart(string userId)
        {
            // Verify user exists
            var user = await GetUserAsync(userId);
            if (user == null) return NotFound("User not found");

            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                cart = await _cartRepository.CreateCartAsync(userId);
            }

            return MapToCartResponseDto(cart);
        }

        [HttpPost("{userId}/items")]
        public async Task<ActionResult<CartResponseDto>> AddToCart(
            string userId,
            [FromBody] CartItemRequestDto request)
        {
            if (request.Quantity <= 0)
                return BadRequest("Quantity must be greater than zero");

            // Verify user exists
            var user = await GetUserAsync(userId);
            if (user == null) return NotFound("User not found");

            // Get product details
            ProductDto? product = await GetProductAsync(request.ProductId);
            if (product == null) return NotFound("Product not found");

            if (product.StockQuantity < request.Quantity)
                return BadRequest("Insufficient stock");

            // Get or create cart
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                cart = await _cartRepository.CreateCartAsync(userId);
            }

            // Add item to cart
            await _cartRepository.AddItemToCartAsync(
                cart.Id,
                request.ProductId,
                request.Quantity,
                product.Price);

            // Return updated cart
            var updatedCart = await _cartRepository.GetCartByUserIdAsync(userId);
            return Ok(MapToCartResponseDto(updatedCart));
        }

        [HttpDelete("{userId}/items/{productId}")]
        public async Task<IActionResult> RemoveFromCart(string userId, string productId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null) return NotFound("Cart not found");

            var result = await _cartRepository.RemoveItemFromCartAsync(cart.Id, productId);
            if (!result) return NotFound("Item not found in cart");

            return NoContent();
        }

        [HttpPut("{userId}/items/{productId}")]
        public async Task<IActionResult> UpdateQuantity(
            string userId,
            string productId,
            [FromBody] UpdateQuantityRequestDto request)
        {
            if (request.NewQuantity <= 0)
                return BadRequest("Quantity must be greater than zero");

            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null) return NotFound("Cart not found");

            // Verify product exists and has sufficient stock
            var product = await GetProductAsync(productId);
            if (product == null) return NotFound("Product not found");
            if (product.StockQuantity < request.NewQuantity)
                return BadRequest("Insufficient stock");

            var result = await _cartRepository.UpdateItemQuantityAsync(
                cart.Id,
                productId,
                request.NewQuantity);

            if (!result) return NotFound("Item not found in cart");

            return NoContent();
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> ClearCart(string userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null) return NotFound("Cart not found");

            var result = await _cartRepository.ClearCartAsync(cart.Id);
            if (!result) return NotFound();

            return NoContent();
        }

        private async Task<UserDto?> GetUserAsync(string userId)
        {
            try
            {
                return await _cartRepository.GetUserAsync(userId);
            }
            catch
            {
                return null;
            }
        }

        private async Task<ProductDto?> GetProductAsync(string productId)
        {
            try
            {
                return await _cartRepository.GetProductAsync(productId);
            }
            catch
            {
                return null;
            }
        }

        private CartResponseDto MapToCartResponseDto(Cart cart)
        {
            return new CartResponseDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                CreatedAt = cart.CreatedAt,
                UpdatedAt = cart.UpdatedAt,
                Items = cart.Items.Select(i => new CartItemResponseDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    PriceAtTimeOfAddition = i.PriceAtTimeOfAddition,
                    AddedAt = i.AddedAt
                }).ToList()
            };
        }
    }
}