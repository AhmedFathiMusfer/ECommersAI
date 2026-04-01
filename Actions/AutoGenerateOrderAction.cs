using System.Threading.Tasks;
using ECommersAI.DTOs.Order;
using ECommersAI.Services.Interfaces;

namespace ECommersAI.Actions
{
    public class AutoGenerateOrderAction
    {
        private readonly IOrderService _orderService;

        public AutoGenerateOrderAction(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<OrderDto> ExecuteAsync(AutoGenerateOrderRequest request)
        {
            return await _orderService.AutoGenerateAsync(request);
        }
    }
}
