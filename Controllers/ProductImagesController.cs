using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommersAI.DTOs.Product;
using ECommersAI.Models.Entities;
using ECommersAI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ECommersAI.Controllers
{
    [ApiController]
    [Route("product-images")]
    public class ProductImagesController : ControllerBase
    {
        private readonly IRepository<ProductImage> _productImageRepository;

        public ProductImagesController(IRepository<ProductImage> productImageRepository)
        {
            _productImageRepository = productImageRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductImageDto>>> GetAll()
        {
            var images = await _productImageRepository.GetAllAsync();
            return Ok(images.Select(i => new ProductImageDto
            {
                Id = i.Id,
                ImageUrl = i.ImageUrl,
                IsMain = i.IsMain
            }).ToList());
        }

        [HttpPost]
        public async Task<ActionResult<ProductImageDto>> Create(CreateProductImageStandaloneRequest request)
        {
            var now = DateTime.UtcNow;
            var entity = new ProductImage
            {
                Id = Guid.NewGuid(),
                ProductId = request.ProductId,
                ImageUrl = request.ImageUrl,
                IsMain = request.IsMain,
                CreatedAt = now,
                UpdatedAt = now
            };

            await _productImageRepository.AddAsync(entity);
            return Ok(new ProductImageDto
            {
                Id = entity.Id,
                ImageUrl = entity.ImageUrl,
                IsMain = entity.IsMain
            });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _productImageRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            await _productImageRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
