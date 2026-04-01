using AutoMapper;
using ECommersAI.DTOs.Message;
using ECommersAI.DTOs.Order;
using ECommersAI.DTOs.Product;
using ECommersAI.DTOs.Trader;
using ECommersAI.Models.Entities;

namespace ECommersAI.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Trader, TraderDto>();

            CreateMap<ProductImage, ProductImageDto>();
            CreateMap<ProductAttribute, ProductAttributeDto>();
            CreateMap<Product, ProductDto>();

            CreateMap<OrderItem, OrderItemDto>();
            CreateMap<Order, OrderDto>()
                .ForMember(d => d.Items, o => o.MapFrom(s => s.OrderItems));

            CreateMap<Message, MessageDto>();
        }
    }
}
