using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommersAI.DTOs.Product;
using ECommersAI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommersAI.Controllers
{
    [ApiController]
    [Route("products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductDto>>> GetAll()
        {
            return Ok(await _productService.GetAllAsync());
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProductDto>> GetById(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);
            return product == null ? NotFound() : Ok(product);
        }

        [HttpPost("{traderId:guid}")]
        public async Task<ActionResult<ProductDto>> Create(Guid traderId, CreateProductRequest request)
        {
            var product = await _productService.CreateAsync(traderId, request);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateProductRequest request)
        {
            var ok = await _productService.UpdateAsync(id, request);
            return ok ? NoContent() : NotFound();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ok = await _productService.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<ProductSearchResultDto>>> Search(
            [FromQuery] Guid traderId,
            [FromQuery] string query,
            [FromQuery] string currency = "USD",
            [FromQuery] int topK = 5)
        {
            var result = await _productService.SearchByQueryAsync(traderId, query, currency, topK);
            return Ok(result);
        }
    }
}
