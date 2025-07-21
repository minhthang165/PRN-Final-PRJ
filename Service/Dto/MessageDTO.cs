namespace PRN_Final_Project.Service.Dto
{
    public class MessageDTO
    {
        public int messageId { get; set; }
        public int createdBy { get; set; }
        public int conversationId { get; set; }
        public string messageContent { get; set; }
        public string messageType { get; set; }
        public string messageStatus { get; set; }
    }
}
