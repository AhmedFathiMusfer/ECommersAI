using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommersAI.DTOs.Product;
using ECommersAI.Models.Entities;
using ECommersAI.Repositories;
using ECommersAI.Repositories.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommersAI.Controllers
{
    [ApiController]
    [Route("product-attributes")]
    public class ProductAttributesController : ControllerBase
    {
        private readonly IProductAttributeRepository _productAttributeRepository;

        public ProductAttributesController(IProductAttributeRepository productAttributeRepository)
        {
            _productAttributeRepository = productAttributeRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductAttributeDto>>> GetAll()
        {
            var attrs = await _productAttributeRepository.GetAllAsync();
            return Ok(attrs.Select(a => new ProductAttributeDto
            {
                Id = a.Id,
                AttributeName = a.AttributeName,
                AttributeValue = a.AttributeValue
            }).ToList());
        }

        [HttpPost]
        public async Task<ActionResult<ProductAttributeDto>> Create(CreateProductAttributeStandaloneRequest request)
        {
            var entity = new ProductAttribute
            {
                Id = Guid.NewGuid(),
                ProductId = request.ProductId,
                AttributeName = request.AttributeName,
                AttributeValue = request.AttributeValue
            };

            await _productAttributeRepository.AddAsync(entity);
            return Ok(new ProductAttributeDto
            {
                Id = entity.Id,
                AttributeName = entity.AttributeName,
                AttributeValue = entity.AttributeValue
            });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _productAttributeRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            await _productAttributeRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
