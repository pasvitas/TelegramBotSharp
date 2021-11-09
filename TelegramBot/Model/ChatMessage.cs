using System;
namespace TelegramBot.Model
{
    public struct ChatMessage
    {
        public ChatMessage(String Source, long ChatId, String Message)
        {
            this.Source = Source;
            this.ChatId = ChatId;
            this.Message = Message;
        }

        public String Source { get; set; }
        public long ChatId { get; set; }
        public String Message { get; set; }
    }
}
