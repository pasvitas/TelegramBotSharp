using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramBotSharp.Repository.Entity
{
    public class CommandEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? CommandId { get; set; }
        public string CommandName { get; set; }
        public string SourceNames { get; set; }
        public Boolean IsScript { get; set; }
        public string commandAnswer { get; set; }
        public string ScriptName { get; set; }

    }
}
