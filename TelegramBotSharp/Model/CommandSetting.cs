using System;
using System.Collections;
using System.Collections.Generic;

namespace TelegramBotSharp
{
    public struct CommandSetting
    {
        private string CommandName { get; set; }
        private IEnumerable<string> SourceNames { get; set; }
        private Boolean IsScript { get; set; }
        private string commandAnswer { get; set; }
        private string ScriptName { get; set; }
    }
}
