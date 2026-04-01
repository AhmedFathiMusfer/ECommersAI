using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommersAI.DTOs.Product;

namespace ECommersAI.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(Guid id);
        Task<ProductDto> CreateAsync(Guid traderId, CreateProductRequest request);
        Task<bool> UpdateAsync(Guid id, UpdateProductRequest request);
        Task<bool> DeleteAsync(Guid id);
        Task<List<ProductSearchResultDto>> SearchByQueryAsync(Guid traderId, string query, string currency, int topK = 5);
        Task UpsertProductVectorAsync(Guid productId);
    }
}
