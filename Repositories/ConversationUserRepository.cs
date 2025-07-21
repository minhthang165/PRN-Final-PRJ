using Microsoft.EntityFrameworkCore;
using PRN_Final_Project.Business.Data;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Repositories.Interface;
using PRN_Final_Project.Service.Dto;

namespace PRN_Final_Project.Repositories
{
    public class ConversationUserRepository : IConversationUserRepository
    {
        private readonly PRNDbContext _context;

        public ConversationUserRepository(PRNDbContext context)
        {
            _context = context;
        }

        public async Task<Conversation_user> AddUserToConversationAsync(int userId, int conversationId)
        {
            var user = await _context.users.FindAsync(userId)
                ?? throw new KeyNotFoundException($"User with ID {userId} not found.");

            var conversation = await _context.Conversations.FindAsync(conversationId)
                ?? throw new KeyNotFoundException($"Conversation with ID {conversationId} not found.");

            var conversationUser = new Conversation_user
            {
                user_id = userId,
                conversation_id = conversationId,
                is_active = true,
                created_at = DateTime.UtcNow
            };

            await _context.Conversation_users.AddAsync(conversationUser);
            await _context.SaveChangesAsync();
            return _context.Conversation_users
                .Include(cu => cu.user)
                .Include(cu => cu.conversation)
                .FirstOrDefault(cu => cu.user_id == userId && cu.conversation_id == conversationId)
                ?? throw new KeyNotFoundException($"User {userId} is not part of conversation {conversationId}.");
        }

        public async Task<List<Conversation>> GetConversationsByUserIdAsync(int userId)
        {
            // Check if user exists
            var userExists = await _context.users.AnyAsync(u => u.id == userId);
            if (!userExists)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }

            // Get conversations for this user
            var conversations = await _context.Conversation_users
                .Where(cu => cu.user_id == userId && cu.is_active == true)
                .Include(cu => cu.conversation)
                .Select(cu => cu.conversation)
                .Where(c => c.is_active == true)
                .ToListAsync();

            // Handle one-to-one conversations to set proper names and avatars
            foreach (var conversation in conversations)
            {
                if (conversation.type == "OneToOne")
                {
                    // Find the other user in this one-to-one conversation
                    var otherUser = await _context.Conversation_users
                        .Where(cu => cu.conversation_id == conversation.conversation_id && cu.user_id != userId)
                        .Include(cu => cu.user)
                        .Select(cu => cu.user)
                        .FirstOrDefaultAsync();

                    if (otherUser != null)
                    {
                        conversation.conversation_name = $"{otherUser.first_name} {otherUser.last_name}";
                        conversation.conversation_avatar = otherUser.avatar_path;
                    }
                }
            }

            return conversations;
        }

        public async Task<List<Conversation_user>> GetUsersInConversationAsync(int conversationId)
        {
            // Check if conversation exists
            var conversationExists = await _context.Conversations.AnyAsync(c => c.conversation_id == conversationId);
            if (!conversationExists)
            {
                throw new KeyNotFoundException($"Conversation with ID {conversationId} not found.");
            }

            // Get users in this conversation
            return await _context.Conversation_users
                .Where(cu => cu.conversation_id == conversationId && cu.is_active == true)
                .Include(cu => cu.user)
                .ToListAsync();
        }

        public async Task<Conversation_user> RemoveUserFromConversationAsync(int userId, int conversationId)
        {
            var conversationUser = await _context.Conversation_users
                .FirstOrDefaultAsync(cu => cu.user_id == userId && cu.conversation_id == conversationId)
                ?? throw new KeyNotFoundException($"User {userId} is not part of conversation {conversationId}.");

            // Soft delete approach
            conversationUser.is_active = false;
            conversationUser.deleted_at = DateTime.UtcNow;

            _context.Conversation_users.Update(conversationUser);
            await _context.SaveChangesAsync();
            return await _context.Conversation_users
                .Include(cu => cu.user)
                .Include(cu => cu.conversation)
                .FirstOrDefaultAsync(cu => cu.user_id == userId && cu.conversation_id == conversationId)
                ?? throw new KeyNotFoundException($"User {userId} is not part of conversation {conversationId}.");
        }
    }
}