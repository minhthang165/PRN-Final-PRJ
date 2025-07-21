using Microsoft.EntityFrameworkCore;
using PRN_Final_Project.Business.Data;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Repositories.Interface;
using PRN_Final_Project.Service.Dto;

namespace PRN_Final_Project.Repositories
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly PRNDbContext _context;
        public ConversationRepository(PRNDbContext context)
        {
            _context = context;
        }

        public async Task<Conversation> CreateConversationAsync(ConversationDTO conversationDTO)
        {
            var conversation = new Conversation
            {
                conversation_name = conversationDTO.ConversationName,
                conversation_avatar = conversationDTO.ConversationAvatar,
                type = conversationDTO.Type,
                is_active = true,
                created_at = DateTime.UtcNow
            };

            await _context.Conversations.AddAsync(conversation);
            await _context.SaveChangesAsync();
            return conversation;
        }

        public async Task<Conversation> DeleteConversationAsync(int conversationId)
        {
            var conversation = await _context.Conversations.FindAsync(conversationId);
            if (conversation == null)
            {
                throw new KeyNotFoundException($"Conversation with ID {conversationId} not found");
            }

            // Soft delete
            conversation.is_active = false;
            conversation.deleted_at = DateTime.UtcNow;

            _context.Conversations.Update(conversation);
            await _context.SaveChangesAsync();
            return conversation;
        }

        public async Task<Conversation> findOneToOneConversationAsync(int userId1, int userId2)
        {
            // Find all one-to-one conversations that userId1 participates in
            var userOneConversations = await _context.Conversation_users
                .Where(cu => cu.user_id == userId1 && cu.is_active == true)
                .Include(cu => cu.conversation)
                .Where(cu => cu.conversation.type == "OneToOne" && cu.conversation.is_active == true)
                .Select(cu => cu.conversation_id)
                .ToListAsync();

            // Find one-to-one conversations where userId2 also participates
            var conversation = await _context.Conversation_users
                .Where(cu => cu.user_id == userId2 && cu.is_active == true && userOneConversations.Contains(cu.conversation_id))
                .Include(cu => cu.conversation)
                .Select(cu => cu.conversation)
                .FirstOrDefaultAsync();

            if (conversation != null)
            {
                // Find other user (userId2) to set conversation name and avatar
                var otherUser = await _context.users.FindAsync(userId2);
                if (otherUser != null)
                {
                    conversation.conversation_name = $"{otherUser.first_name} {otherUser.last_name}";
                    conversation.conversation_avatar = otherUser.avatar_path;
                }
            }

            return conversation;
        }

        public async Task<Conversation> GetConversationByIdAsync(int conversationId)
        {
            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => c.conversation_id == conversationId && c.is_active == true);

            if (conversation == null)
            {
                throw new KeyNotFoundException($"Conversation with ID {conversationId} not found");
            }

            return conversation;
        }

        public async Task<List<Conversation>> GetConversationsByUserIdAsync(int userId)
        {
            return await _context.Conversation_users
                .Where(cu => cu.user_id == userId && cu.is_active == true)
                .Include(cu => cu.conversation)
                .Where(cu => cu.conversation.is_active == true)
                .Select(cu => cu.conversation)
                .ToListAsync();
        }

        public async Task<Conversation> UpdateConversationAsync(ConversationDTO conversationDTO)
        {
            var conversation = await _context.Conversations.FindAsync(conversationDTO.Conversation_id);
            if (conversation == null)
            {
                throw new KeyNotFoundException($"Conversation with ID {conversationDTO.Conversation_id} not found");
            }

            // Update only the non-null properties
            if (conversationDTO.ConversationName != null)
            {
                conversation.conversation_name = conversationDTO.ConversationName;
            }

            if (conversationDTO.ConversationAvatar != null)
            {
                conversation.conversation_avatar = conversationDTO.ConversationAvatar;
            }

            conversation.is_active = conversationDTO.Is_Active;
            conversation.updated_at = DateTime.UtcNow;

            _context.Conversations.Update(conversation);
            await _context.SaveChangesAsync();
            return conversation;
        }
    }
}