using System;
using System.Collections.Generic;
using TelegramBotSharp.Repository.Entity;

namespace TelegramBotSharp.Repository
{
    public interface ICommandsRepository
    {
        public int SaveCommand(CommandEntity commandEntity);
        public CommandEntity GetCommandById(int id);
        public List<CommandEntity> GetAllCommands();
        public void UpdateCommandEntity(CommandEntity commandEntity);
        public void DeleteCommandEntity(int id);
        public List<CommandEntity> FindBySourceNames(string sourceNames);
        public List<CommandEntity> FindByTrigger(string trigger);
    }
}
