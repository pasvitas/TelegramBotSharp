using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TelegramBotSharp.TelegramBot;
using TelegramBotSharp.Service;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TelegramBotSharp.Repository;
using TelegramBot.Model;
using System.Collections.Generic;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace TelegramBot.Bot
{
    public class BotConnector : IBot, IHostedService
    {

        private readonly Dictionary<string, IProducer<Null, string>> _producers = new Dictionary<string, IProducer<Null, string>>();

        private readonly IServiceScopeFactory _services;

        private readonly ILogger<BotConnector> _logger;

        public BotConnector(IServiceScopeFactory services, ILogger<BotConnector> logger)
        {
            this._logger = logger;
            this._services = services;

        }

        public async Task SendMessageToChat(ChatMessage chatMessage)
        {
            IProducer<Null, string> producer;
            if (_producers.ContainsKey(chatMessage.Source))
            {
                producer = _producers[chatMessage.Source];
            }
            else
            {
                var producerConfig = new ProducerConfig
                {
                    BootstrapServers = "localhost:9092",
                    ClientId = "bot.receiver."+chatMessage.Source,
                };
                producer = new ProducerBuilder<Null, string>(producerConfig).Build();
                _producers[chatMessage.Source] = producer;
            }
            await producer.ProduceAsync("sendMessage", new Message<Null, string> { Value = JsonConvert.SerializeObject(chatMessage) });
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var scope = _services.CreateScope();
            
            var dbContext = scope.ServiceProvider.GetService<BotDatabaseContext>();
            var repository = new CommandRepository(dbContext);
            var chatService = new ChatService(repository);

            Task consumerTask = new Task(() =>
            {
                var consumerConfig = new ConsumerConfig
                {
                    BootstrapServers = "localhost:9092",
                    GroupId = "bot.processor",
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    AllowAutoCreateTopics = true
                };

                using (var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
                {
                    consumer.Subscribe("receiveMessage");

                    while (true)
                    {
                        var consumeResult = consumer.Consume(new CancellationToken());
                        ChatMessage chatMessage = JsonConvert.DeserializeObject<ChatMessage>(consumeResult.Message.Value);
                        chatService.processMessage(chatMessage);
                    }
                }
            });
            consumerTask.Start();

            chatService.registerBot(this);
            _logger.LogInformation("Scope inited");
         

            _logger.LogInformation("Bot init");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
