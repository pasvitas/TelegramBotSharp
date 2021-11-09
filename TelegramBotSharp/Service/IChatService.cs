using System;
using TelegramBot.Model;
using TelegramBotSharp.TelegramBot;

namespace TelegramBotSharp.Service
{
    public interface IChatService
    {
        void processMessage(ChatMessage chatMessage);
        void registerBot(IBot bot);
    }
}
