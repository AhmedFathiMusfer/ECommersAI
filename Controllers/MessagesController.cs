using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommersAI.DTOs.Message;
using ECommersAI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommersAI.Controllers
{
    [ApiController]
    [Route("messages")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet]
        public async Task<ActionResult<List<MessageDto>>> GetAll()
        {
            return Ok(await _messageService.GetAllAsync());
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MessageDto>> GetById(Guid id)
        {
            var message = await _messageService.GetByIdAsync(id);
            return message == null ? NotFound() : Ok(message);
        }
    }
}
