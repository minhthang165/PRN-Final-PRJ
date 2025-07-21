namespace PRN_Final_Project.Service.Dto
{
    public class ConversationDTO
    {
        public int Conversation_id { get; set; }
        public string? ConversationName { get; set; }
        public string? ConversationAvatar { get; set; }
        public string? Type { get; set; }
        public bool Is_Active { get; set; }
    }
}
