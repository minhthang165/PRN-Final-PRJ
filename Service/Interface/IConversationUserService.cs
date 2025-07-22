using PRN_Final_Project.Business.Entities;

namespace PRN_Final_Project.Service.Interface
{
    public interface IConversationUserService
    {
        Task<List<Conversation_user>> GetUsersInConversationAsync(int conversationId);
        Task<Conversation_user> AddUserToConversationAsync(int userId, int conversationId);
        Task<Conversation_user> RemoveUserFromConversationAsync(int userId, int conversationId);
        Task<List<Conversation>> GetConversationsByUserIdAsync(int userId);
    }
}
