using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommersAI.Actions;
using ECommersAI.DTOs.Order;
using ECommersAI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommersAI.Controllers
{
    [ApiController]
    [Route("orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly AutoGenerateOrderAction _autoGenerateOrderAction;

        public OrdersController(IOrderService orderService, AutoGenerateOrderAction autoGenerateOrderAction)
        {
            _orderService = orderService;
            _autoGenerateOrderAction = autoGenerateOrderAction;
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderDto>>> GetAll()
        {
            return Ok(await _orderService.GetAllAsync());
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OrderDto>> GetById(Guid id)
        {
            var order = await _orderService.GetByIdAsync(id);
            return order == null ? NotFound() : Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> Create(CreateOrderRequest request)
        {
            var order = await _orderService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }

        [HttpPost("auto-generate")]
        public async Task<ActionResult<OrderDto>> AutoGenerate(AutoGenerateOrderRequest request)
        {
            var order = await _autoGenerateOrderAction.ExecuteAsync(request);
            return Ok(order);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ok = await _orderService.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
