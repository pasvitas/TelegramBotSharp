using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotSharp;
using TelegramBotSharp.TelegramBot;
using TelegramBotSharp.Service;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TelegramBotSharp.Repository;

namespace TelegramBot.Bot
{
    public class TelegramBotBot : IBot, IHostedService
    {

        private IChatService _chatService;

        private readonly IServiceScopeFactory _services;

        private readonly ILogger<TelegramBotBot> _logger;

        private readonly TelegramBotClient _botClient = new TelegramBotClient("2099109615:AAFeKA2uca2N96VOY7t8wh7uHUGaQ6q5b_E");

        public TelegramBotBot(IServiceScopeFactory services, ILogger<TelegramBotBot> logger)
        {
            this._logger = logger;
            this._services = services;
        }

        public async Task SendMessageToChat(long chatId, String text)
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: text
            );
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using var cts = new CancellationTokenSource();

            _botClient.StartReceiving(
                new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync),
                cts.Token);

            var scope = _services.CreateScope();
            
            var dbContext = scope.ServiceProvider.GetService<BotDatabaseContext>();
            var repository = new CommandRepository(dbContext);
            var chatService = new ChatService(repository);

            _chatService = chatService;
            chatService.registerBot(this);
            _logger.LogInformation("Scope inited");
         

            _logger.LogInformation("Bot init");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            _logger.LogError(ErrorMessage, exception);
            return Task.CompletedTask;
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type != UpdateType.Message)
                return;
            if (update.Message.Type != MessageType.Text)
                return;

            var chatId = update.Message.Chat.Id;

            _logger.LogInformation($"Получено '{update.Message.Text}' в чате {chatId}.");

            _chatService.processMessage(new Model.ChatMessage(chatId, update.Message.Text));
        }


    }
}
