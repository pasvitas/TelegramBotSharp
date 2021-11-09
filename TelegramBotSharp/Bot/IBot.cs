using System;
using System.Threading.Tasks;

namespace TelegramBotSharp.TelegramBot
{
    public interface IBot
    {
        Task SendMessageToChat(long chatId, String text);
    }
}
