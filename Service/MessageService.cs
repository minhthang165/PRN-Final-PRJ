using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Repositories.Interface;
using PRN_Final_Project.Service.Dto;
using PRN_Final_Project.Service.Interface;

namespace PRN_Final_Project.Service
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;

        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }
        public Task<Message> addMessage(MessageDTO message)
        {
            return _messageRepository.addMessage(message);
        }

        public Task<Message> deleteMessage(int messageId)
        {
            return _messageRepository.deleteMessage(messageId);
        }

        public Task<Message> GetMessageByIdAsync(int messageId)
        {
            return _messageRepository.GetMessageByIdAsync(messageId);
        }

        public Task<List<Message>> getMessageInConversation(int conversationId)
        {
            return _messageRepository.getMessageInConversation(conversationId);
        }

        public Task<Message> updateMessage(MessageDTO message)
        {
            return _messageRepository.updateMessage(message);
        }
    }
}
