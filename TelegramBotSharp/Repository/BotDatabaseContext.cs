using System;
using Microsoft.EntityFrameworkCore;
using TelegramBotSharp.Repository.Entity;

namespace TelegramBotSharp.Repository
{
    public class BotDatabaseContext : DbContext
    {
        public BotDatabaseContext(DbContextOptions<BotDatabaseContext> options) : base(options)
        {
        }

        public DbSet<CommandEntity> CommandEntities { get; set; }
        //public DbSet<UserDataEntity> UserDataEntities { get; set; }
    }
}
