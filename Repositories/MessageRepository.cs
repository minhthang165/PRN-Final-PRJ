using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using PRN_Final_Project.Business.Data;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Repositories.Interface;
using PRN_Final_Project.Service.Dto;

namespace PRN_Final_Project.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly PRNDbContext _context;

        public MessageRepository(PRNDbContext context)
        {
            _context = context;
        }

        public async Task<Message> addMessage(MessageDTO messageDTO)
        {
            var message = new Message
            {
                conversation_id = messageDTO.conversationId,
                message_content = messageDTO.messageContent,
                message_type = messageDTO.messageType,
                status = messageDTO.messageStatus,
                created_by = messageDTO.createdBy,
                created_at = DateTime.UtcNow,
                is_active = true
            };
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();

            // Reload the message with its conversation included
            return await _context.Messages
                .Include(m => m.conversation)
                .Include(u => u.sender)
                .FirstOrDefaultAsync(m => m.message_id == message.message_id);
        }

        public async Task<Message> deleteMessage(int messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message == null)
                throw new KeyNotFoundException($"Message with ID {messageId} not found.");

            message.status = "DELETED";
            message.is_active = false;
            message.deleted_at = DateTime.UtcNow;

            _context.Messages.Update(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<Message> GetMessageByIdAsync(int messageId)
        {
            return await _context.Messages
                .FirstOrDefaultAsync(m => m.message_id == messageId);
        }

        public async Task<List<Message>> getMessageInConversation(int conversationId)
        {
            return await _context.Messages
                .Include(m => m.sender) 
                .Where(m => m.conversation_id == conversationId && m.is_active == true)
                .OrderBy(m => m.created_at)
                .ToListAsync();
        }

        public async Task<Message> updateMessage(MessageDTO messageDTO)
        {
            var existingMessage = await _context.Messages.FindAsync(messageDTO.messageId);
            if (existingMessage == null)
                throw new KeyNotFoundException($"Message with ID {messageDTO.messageId} not found.");

            existingMessage.message_content = messageDTO.messageContent;
            existingMessage.updated_at = DateTime.UtcNow;

            _context.Messages.Update(existingMessage);
            await _context.SaveChangesAsync();
            return existingMessage;
        }
    }
}
