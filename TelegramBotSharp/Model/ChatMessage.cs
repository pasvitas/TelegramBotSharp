using System;
namespace TelegramBot.Model
{
    public struct ChatMessage
    {
        public ChatMessage(long ChatId, String Message)
        {
            this.ChatId = ChatId;
            this.Message = Message;
        }

        public long ChatId { get; set; }
        public String Message { get; set; }
    }
}
