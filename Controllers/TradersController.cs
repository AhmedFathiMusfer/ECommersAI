using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommersAI.DTOs.Trader;
using ECommersAI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommersAI.Controllers
{
    [ApiController]
    [Route("traders")]
    public class TradersController : ControllerBase
    {
        private readonly ITraderService _traderService;

        public TradersController(ITraderService traderService)
        {
            _traderService = traderService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TraderDto>>> GetAll()
        {
            return Ok(await _traderService.GetAllAsync());
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TraderDto>> GetById(Guid id)
        {
            var trader = await _traderService.GetByIdAsync(id);
            if (trader == null)
            {
                return NotFound();
            }

            return Ok(trader);
        }

        [HttpPost]
        public async Task<ActionResult<TraderDto>> Create(CreateTraderRequest request)
        {
            var trader = await _traderService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = trader.Id }, trader);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateTraderRequest request)
        {
            var ok = await _traderService.UpdateAsync(id, request);
            return ok ? NoContent() : NotFound();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ok = await _traderService.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
