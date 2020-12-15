using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public MessageRepository(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return await _context.Groups
                .Include(x => x.Connections)
                .Where(x => x.Connections.Any(t => t.ConnectionId == connectionId))
                .FirstOrDefaultAsync();
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages
                .Include(x => x.Sender)
                .Include(x => x.Recipient)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups
                .Include(x => x.Connections)
                .FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages
                .OrderByDescending(x => x.MessageSent)
                .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(x => x.RecipientUsername == messageParams.Username && !x.RecipientDeleted),
                "Outbox" => query.Where(x => x.SenderUsername == messageParams.Username && !x.SenderDeleted),
                _ => query.Where(x => x.RecipientUsername == messageParams.Username && !x.RecipientDeleted && x.DateRead == null)
            };

            return await PagedList<MessageDto>.CreateAsync(query, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages = await _context.Messages
                .Where(x => x.Recipient.UserName == recipientUsername && !x.RecipientDeleted && x.Sender.UserName == currentUsername
                || x.Recipient.UserName == currentUsername && x.Sender.UserName == recipientUsername && !x.SenderDeleted)
                .OrderBy(x => x.MessageSent)
                .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var unreadMessages = messages.Where(x => x.DateRead == null && x.RecipientUsername == currentUsername).ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                    message.DateRead = DateTime.UtcNow;
            }

            return messages;
        }

        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }
    }
}
