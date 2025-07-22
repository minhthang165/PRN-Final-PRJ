using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Repositories.Interface;
using PRN_Final_Project.Service.Dto;
using PRN_Final_Project.Service.Interface;

namespace PRN_Final_Project.Service
{
    public class ConversationService : IConversationService
    {
        private readonly IConversationRepository _conversationRepository;

        public ConversationService(IConversationRepository conversationRepository)
        {
            _conversationRepository = conversationRepository;
        }

        public Task<Conversation> CreateConversationAsync(ConversationDTO conversation)
        {
            return _conversationRepository.CreateConversationAsync(conversation);
        }

        public Task<Conversation> DeleteConversationAsync(int conversationId)
        {
            return _conversationRepository.DeleteConversationAsync(conversationId);
        }

        public Task<Conversation> findOneToOneConversationAsync(int userId1, int userId2)
        {
            return _conversationRepository.findOneToOneConversationAsync(userId1, userId2);
        }

        public Task<Conversation> GetConversationByIdAsync(int conversationId)
        {
            return _conversationRepository.GetConversationByIdAsync(conversationId);
        }

        public Task<List<Conversation>> GetConversationsByUserIdAsync(int userId)
        {
            return _conversationRepository.GetConversationsByUserIdAsync(userId);
        }

        public Task<Conversation> UpdateConversationAsync(ConversationDTO conversation)
        {
            return _conversationRepository.UpdateConversationAsync(conversation);
        }
    }
}
