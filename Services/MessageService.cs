using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ECommersAI.DTOs.Message;
using ECommersAI.Models.Entities;
using ECommersAI.Repositories;
using ECommersAI.Repositories.interfaces;
using ECommersAI.Services.Interfaces;

namespace ECommersAI.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public MessageService(IMessageRepository messageRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        public async Task<List<MessageDto>> GetAllAsync()
        {
            var messages = await _messageRepository.GetAllAsync();
            return messages.Select(m => _mapper.Map<MessageDto>(m)).ToList();
        }

        public async Task<MessageDto?> GetByIdAsync(Guid id)
        {
            var message = await _messageRepository.GetByIdAsync(id);
            return message == null ? null : _mapper.Map<MessageDto>(message);
        }

        public async Task<Message> CreateAsync(Guid traderId, string customerPhone, string messageType, string content, string aiResponse)
        {
            var message = new Message
            {
                Id = Guid.NewGuid(),
                TraderId = traderId,
                CustomerPhone = customerPhone,
                MessageType = messageType,
                Content = content,
                AIResponse = aiResponse,
                CreatedAt = DateTime.UtcNow
            };

            await _messageRepository.AddAsync(message);
            return message;
        }
        public async Task<List<MessageDto>> GetHistoryMessage()
        {
            var messages = await _messageRepository.GetAllAsync();
            return messages.Select(m => _mapper.Map<MessageDto>(m)).ToList();
        }

        public async Task<List<Message>> GetMessageHistoryAsync(Guid traderId, string customerPhone, CancellationToken cancellationToken)
        {
            return await _messageRepository.GetHistoryMessagesAsync(traderId, customerPhone, cancellationToken);
        }
    }
}
