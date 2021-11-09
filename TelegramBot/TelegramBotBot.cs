using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.Hosting;
using Confluent.Kafka;
using TelegramBot.Model;
using Newtonsoft.Json;

namespace TelegramBot.Bot
{
    public class TelegramBotBot : IHostedService
    {

        private readonly ILogger<TelegramBotBot> _logger;

        private readonly IProducer<Null, String> _producer;

        private readonly TelegramBotClient _botClient = new TelegramBotClient("2099109615:AAFeKA2uca2N96VOY7t8wh7uHUGaQ6q5b_E");

        public TelegramBotBot(ILogger<TelegramBotBot> logger)
        {
            this._logger = logger;
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = "localhost:9092",
                ClientId = "bot.processor",
            };
            _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
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

            Task consumerTask = new Task(async () =>
            {
                var consumerConfig = new ConsumerConfig
                {
                    BootstrapServers = "localhost:9092",
                    GroupId = "bot.receiver.telegram",
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    AllowAutoCreateTopics = true
                };

                using (var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
                {
                    consumer.Subscribe("sendMessage");

                    while (true)
                    {
                        var consumeResult = consumer.Consume(cancellationToken);
                        ChatMessage chatMessage = JsonConvert.DeserializeObject<ChatMessage>(consumeResult.Message.Value);
                        await SendMessageToChat(chatMessage.ChatId, chatMessage.Message);
                    }

                    consumer.Close();
                }
            });
            consumerTask.Start();
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


            ChatMessage message = new ChatMessage("telegram", chatId, update.Message.Text);

            await _producer.ProduceAsync("receiveMessage", new Message<Null, string> { Value = JsonConvert.SerializeObject(message) });
        }


    }
}
