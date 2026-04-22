using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ECommersAI.DTOs.Order;
using ECommersAI.Models.Entities;
using ECommersAI.Repositories;
using ECommersAI.Repositories.interfaces;
using ECommersAI.Services.Interfaces;

namespace ECommersAI.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IProductRepository _productRepository;
        private readonly IExchangeRateService _exchangeRateService;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            IProductRepository productRepository,
            IExchangeRateService exchangeRateService,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _productRepository = productRepository;
            _exchangeRateService = exchangeRateService;
            _mapper = mapper;
        }

        public async Task<List<OrderDto>> GetAllAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return orders.Select(o => _mapper.Map<OrderDto>(o)).ToList();
        }

        public async Task<OrderDto?> GetByIdAsync(Guid id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            return order == null ? null : _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> CreateAsync(CreateOrderRequest request)
        {
            var products = (await _productRepository.GetAllAsync()).ToDictionary(p => p.Id, p => p);
            var now = DateTime.UtcNow;

            var order = new Order
            {
                Id = Guid.NewGuid(),
                TraderId = request.TraderId,
                CustomerName = request.CustomerName,
                CustomerPhone = request.CustomerPhone,
                CustomerAddress = request.CustomerAddress,
                Currency = request.Currency,
                Status = "Pending",
                CreatedAt = now,
                UpdatedAt = now,
                OrderItems = new List<OrderItem>()
            };

            decimal total = 0;
            foreach (var item in request.Items)
            {
                if (!products.TryGetValue(item.ProductId, out var product))
                {
                    continue;
                }

                var unitPrice = product.PriceUSD;
                var convertedPrice = await _exchangeRateService.ConvertFromUsdAsync(unitPrice * item.Quantity, request.Currency);

                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPriceUSD = unitPrice,
                    ConvertedPrice = convertedPrice
                };

                order.OrderItems.Add(orderItem);
                total += convertedPrice;
            }

            order.TotalPrice = Math.Round(total, 2);

            await _orderRepository.AddAsync(order);
            foreach (var item in order.OrderItems)
            {
                await _orderItemRepository.AddAsync(item);
            }

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return false;
            }

            await _orderRepository.DeleteAsync(id);
            return true;
        }

        public async Task<OrderDto> AutoGenerateAsync(AutoGenerateOrderRequest request)
        {
            var products = (await _productRepository.GetAllAsync())
                .Where(p => p.TraderId == request.TraderId)
                .ToList();

            var matched = products
                .Where(p => request.ConversationText.Contains(p.Name, StringComparison.OrdinalIgnoreCase))
                .Take(3)
                .ToList();

            if (!matched.Any() && products.Any())
            {
                matched.Add(products.First());
            }

            var createRequest = new CreateOrderRequest
            {
                TraderId = request.TraderId,
                CustomerName = request.CustomerName,
                CustomerPhone = request.CustomerPhone,
                CustomerAddress = request.CustomerAddress,
                Currency = request.Currency,
                Items = matched.Select(p => new CreateOrderItemRequest
                {
                    ProductId = p.Id,
                    Quantity = 1
                }).ToList()
            };

            return await CreateAsync(createRequest);
        }
    }
}
