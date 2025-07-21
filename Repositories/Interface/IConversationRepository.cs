using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Service.Dto;

namespace PRN_Final_Project.Repositories.Interface
{
    public interface IConversationRepository
    {
        Task<List<Conversation>> GetConversationsByUserIdAsync(int userId);
        Task<Conversation> CreateConversationAsync(ConversationDTO conversation);
        Task<Conversation> GetConversationByIdAsync(int conversationId);
        Task<Conversation> UpdateConversationAsync(ConversationDTO conversation);
        Task<Conversation> DeleteConversationAsync(int conversationId);
        Task<Conversation> findOneToOneConversationAsync(int userId1, int userId2);
    }
}
