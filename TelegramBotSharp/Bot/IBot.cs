using System;
using System.Threading.Tasks;
using TelegramBot.Model;

namespace TelegramBotSharp.TelegramBot
{
    public interface IBot
    {
        Task SendMessageToChat(ChatMessage chatMessage);
    }
}
