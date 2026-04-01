using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ECommersAI.DTOs.Trader;
using ECommersAI.Models.Entities;
using ECommersAI.Repositories;
using ECommersAI.Services.Interfaces;

namespace ECommersAI.Services
{
    public class TraderService : ITraderService
    {
        private readonly IRepository<Trader> _traderRepository;
        private readonly IMapper _mapper;

        public TraderService(IRepository<Trader> traderRepository, IMapper mapper)
        {
            _traderRepository = traderRepository;
            _mapper = mapper;
        }

        public async Task<List<TraderDto>> GetAllAsync()
        {
            var traders = await _traderRepository.GetAllAsync();
            return traders.Select(t => _mapper.Map<TraderDto>(t)).ToList();
        }

        public async Task<TraderDto?> GetByIdAsync(Guid id)
        {
            var trader = await _traderRepository.GetByIdAsync(id);
            return trader == null ? null : _mapper.Map<TraderDto>(trader);
        }

        public async Task<TraderDto> CreateAsync(CreateTraderRequest request)
        {
            var now = DateTime.UtcNow;
            var trader = new Trader
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                PhoneNumber = request.PhoneNumber,
                WhatsAppId = request.WhatsAppId,
                SubscriptionStatus = request.SubscriptionStatus,
                DefaultCurrency = request.DefaultCurrency,
                CreatedAt = now,
                UpdatedAt = now
            };

            await _traderRepository.AddAsync(trader);
            return _mapper.Map<TraderDto>(trader);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateTraderRequest request)
        {
            var trader = await _traderRepository.GetByIdAsync(id);
            if (trader == null)
            {
                return false;
            }

            trader.Name = request.Name;
            trader.PhoneNumber = request.PhoneNumber;
            trader.WhatsAppId = request.WhatsAppId;
            trader.SubscriptionStatus = request.SubscriptionStatus;
            trader.DefaultCurrency = request.DefaultCurrency;
            trader.UpdatedAt = DateTime.UtcNow;

            await _traderRepository.UpdateAsync(trader);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var trader = await _traderRepository.GetByIdAsync(id);
            if (trader == null)
            {
                return false;
            }

            await _traderRepository.DeleteAsync(id);
            return true;
        }
    }
}
