using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommersAI.DTOs.Trader;

namespace ECommersAI.Services.Interfaces
{
    public interface ITraderService
    {
        Task<List<TraderDto>> GetAllAsync();
        Task<TraderDto?> GetByIdAsync(Guid id);
        Task<TraderDto> CreateAsync(CreateTraderRequest request);
        Task<bool> UpdateAsync(Guid id, UpdateTraderRequest request);
        Task<bool> DeleteAsync(Guid id);
    }
}
