using System;
using System.Collections.Generic;
using System.Linq;
using TelegramBotSharp.Repository.Entity;

namespace TelegramBotSharp.Repository
{
    public class CommandRepository : ICommandsRepository
    {

        private readonly BotDatabaseContext _botDatabaseContext;

        public CommandRepository(BotDatabaseContext context)
        {
            _botDatabaseContext = context;
        }

        public void DeleteCommandEntity(int id)
        { 
            _botDatabaseContext.Remove(GetCommandById(id));
            _botDatabaseContext.SaveChanges();
        }

        public List<CommandEntity> FindBySourceNames(string sourceNames)
        {
            return _botDatabaseContext.CommandEntities
                .Where(entity => entity.SourceNames.Contains(sourceNames))
                .ToList();
        }

        public List<CommandEntity> GetAllCommands()
        {
            return _botDatabaseContext.CommandEntities.ToList();
        }

        public CommandEntity GetCommandById(int id)
        {
            return _botDatabaseContext.Find<CommandEntity>(id);
        }

        public int SaveCommand(CommandEntity commandEntity)
        {
            _botDatabaseContext.Add(commandEntity);
            _botDatabaseContext.SaveChanges();
            return commandEntity.CommandId;
        }

        public void UpdateCommandEntity(CommandEntity commandEntity)
        {
            _botDatabaseContext.Update(commandEntity);
            _botDatabaseContext.SaveChanges();
        }
    }
}
