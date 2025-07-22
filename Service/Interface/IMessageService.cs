using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Service.Dto;

namespace PRN_Final_Project.Service.Interface
{
    public interface IMessageService
    {
        Task<List<Message>> getMessageInConversation(int conversationId);
        Task<Message> addMessage(MessageDTO message);
        Task<Message> deleteMessage(int messageId);
        Task<Message> updateMessage(MessageDTO message);
        Task<Message> GetMessageByIdAsync(int messageId);
    }
}
