using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Repositories.Interface;
using PRN_Final_Project.Service.Interface;

namespace PRN_Final_Project.Service
{
    public class ConversationUserService : IConversationUserService
    {
        private readonly IConversationUserRepository _conversationUserRepository;

        public ConversationUserService(IConversationUserRepository conversationUserRepository)
        {
            _conversationUserRepository = conversationUserRepository;
        }

        public Task<Conversation_user> AddUserToConversationAsync(int userId, int conversationId)
        {
            return _conversationUserRepository.AddUserToConversationAsync(userId, conversationId);
        }

        public Task<List<Conversation>> GetConversationsByUserIdAsync(int userId)
        {
            return _conversationUserRepository.GetConversationsByUserIdAsync(userId);
        }

        public Task<List<Conversation_user>> GetUsersInConversationAsync(int conversationId)
        {
            return _conversationUserRepository.GetUsersInConversationAsync(conversationId);
        }

        public Task<Conversation_user> RemoveUserFromConversationAsync(int userId, int conversationId)
        {
            return _conversationUserRepository.RemoveUserFromConversationAsync(userId, conversationId);
        }
    }
}
