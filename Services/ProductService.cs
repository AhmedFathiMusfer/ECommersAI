using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ECommersAI.DTOs.Product;
using ECommersAI.Features.AI.Embedding;
using ECommersAI.Models.Entities;
using ECommersAI.Repositories;
using ECommersAI.Repositories.interfaces;
using ECommersAI.Services.Interfaces;
using Pgvector;

namespace ECommersAI.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductVectorRepository _productVectorRepository;
        private readonly IEmbeddingService _embeddingService;
        private readonly IExchangeRateService _exchangeRateService;
        private readonly IMapper _mapper;

        public ProductService(
            IProductRepository productRepository,
            IProductVectorRepository productVectorRepository,
            IEmbeddingService embeddingService,
            IExchangeRateService exchangeRateService,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _productVectorRepository = productVectorRepository;
            _embeddingService = embeddingService;
            _exchangeRateService = exchangeRateService;
            _mapper = mapper;
        }

        public async Task<List<ProductDto>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(p => _mapper.Map<ProductDto>(p)).ToList();
        }

        public async Task<ProductDto?> GetByIdAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateAsync(Guid traderId, CreateProductRequest request)
        {
            var now = DateTime.UtcNow;
            var product = new Product
            {
                Id = Guid.NewGuid(),
                TraderId = traderId,
                Name = request.Name,
                Description = request.Description,
                PriceUSD = request.PriceUSD,
                Category = request.Category,
                CreatedAt = now,
                UpdatedAt = now,
                Images = request.Images.Select(i => new ProductImage
                {
                    Id = Guid.NewGuid(),
                    ImageUrl = i.ImageUrl,
                    IsMain = i.IsMain,
                    CreatedAt = now,
                    UpdatedAt = now
                }).ToList(),
                Attributes = request.Attributes.Select(a => new ProductAttribute
                {
                    Id = Guid.NewGuid(),
                    AttributeName = a.AttributeName,
                    AttributeValue = a.AttributeValue
                }).ToList()
            };

            await _productRepository.AddAsync(product);
            await UpsertProductVectorAsync(product.Id);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateProductRequest request)
        {
            var existing = await _productRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return false;
            }

            existing.Name = request.Name;
            existing.Description = request.Description;
            existing.PriceUSD = request.PriceUSD;
            existing.Category = request.Category;
            existing.UpdatedAt = DateTime.UtcNow;

            var now = DateTime.UtcNow;
            existing.Images = request.Images.Select(i => new ProductImage
            {
                Id = Guid.NewGuid(),
                ProductId = existing.Id,
                ImageUrl = i.ImageUrl,
                IsMain = i.IsMain,
                CreatedAt = now,
                UpdatedAt = now
            }).ToList();

            existing.Attributes = request.Attributes.Select(a => new ProductAttribute
            {
                Id = Guid.NewGuid(),
                ProductId = existing.Id,
                AttributeName = a.AttributeName,
                AttributeValue = a.AttributeValue
            }).ToList();

            await _productRepository.UpdateAsync(existing);
            await UpsertProductVectorAsync(existing.Id);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return false;
            }

            await _productRepository.DeleteAsync(id);
            await _productVectorRepository.DeleteAsync(id);
            return true;
        }

        public async Task<List<ProductSearchResultDto>> SearchByQueryAsync(Guid traderId, string query, string currency, int topK = 5)
        {
            var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(query);
            var vectors = await _productVectorRepository.GetAllAsync();
            var products = await _productRepository.GetAllAsync();

            var traderProducts = products.Where(p => p.TraderId == traderId).ToDictionary(p => p.Id, p => p);

            var scored = vectors
                .Where(v => traderProducts.ContainsKey(v.ProductId) && v.Vector != null && v.Vector.ToArray().Length > 0)
                .Select(v => new
                {
                    Product = traderProducts[v.ProductId],
                    Score = CosineSimilarity(queryEmbedding, v.Vector.ToArray())
                })
                .OrderByDescending(x => x.Score)
                .Take(topK)
                .ToList();

            var results = new List<ProductSearchResultDto>();
            foreach (var row in scored)
            {
                var convertedPrice = await _exchangeRateService.ConvertFromUsdAsync(row.Product.PriceUSD, currency);

                results.Add(new ProductSearchResultDto
                {
                    ProductId = row.Product.Id,
                    ProductName = row.Product.Name,
                    Description = row.Product.Description,
                    Category = row.Product.Category,
                    PriceUSD = row.Product.PriceUSD,
                    DisplayPrice = convertedPrice,
                    Currency = currency,
                    SimilarityScore = row.Score
                });
            }

            return results;
        }

        public async Task<List<ProductSearchResultDto>> SearchByVectorQueryAsync(Guid traderId, string query, int topK = 5)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<ProductSearchResultDto>();
            }

            var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(query);
            var queryVector = new Vector(queryEmbedding);

            var scored = _productVectorRepository.SearchNearestByTrader(traderId, queryVector).Select(row => new ProductSearchResultDto
            {
                ProductId = row.ProductId,
                ProductName = row.Product.Name,
                Description = row.Product.Description,
                Category = row.Product.Category,
                PriceUSD = row.Product.PriceUSD,
                DisplayPrice = row.Product.PriceUSD,
                Currency = "USD",
                SimilarityScore = CosineSimilarity(queryEmbedding, row.Vector.ToArray())
            })
            .Take(topK)
            .ToList();

            return scored;
        }

        public async Task UpsertProductVectorAsync(Guid productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return;
            }

            var embeddingText = $"{product.Name}. {product.Description}. Category: {product.Category}";
            var embedding = await _embeddingService.GenerateEmbeddingAsync(embeddingText);

            var existingVector = await _productVectorRepository.GetByIdAsync(productId);
            if (existingVector == null)
            {
                await _productVectorRepository.AddAsync(new ProductVector
                {
                    ProductId = productId,
                    Vector = new Vector(embedding)
                });
                return;
            }

            existingVector.Vector = new Vector(embedding);
            await _productVectorRepository.UpdateAsync(existingVector);
        }

        private static double CosineSimilarity(float[] left, float[] right)
        {
            if (left.Length == 0 || right.Length == 0)
            {
                return 0d;
            }

            var length = Math.Min(left.Length, right.Length);
            double dot = 0;
            double normLeft = 0;
            double normRight = 0;

            for (var i = 0; i < length; i++)
            {
                dot += left[i] * right[i];
                normLeft += left[i] * left[i];
                normRight += right[i] * right[i];
            }

            if (normLeft == 0 || normRight == 0)
            {
                return 0d;
            }

            return dot / (Math.Sqrt(normLeft) * Math.Sqrt(normRight));
        }
    }
}
