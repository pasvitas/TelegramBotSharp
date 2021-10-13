using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TelegramBotSharp.Repository;
using TelegramBotSharp.Repository.Entity;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TelegramBotSharp.Controllers
{
    [Route("api/[controller]")]
    public class CommandsController : Controller
    {

        private readonly ICommandsRepository _commandsRepository;

        public CommandsController(ICommandsRepository commandsRepository)
        {
            _commandsRepository = commandsRepository;
        }


        // GET: api/values
        [HttpGet]
        public IEnumerable<CommandEntity> Get()
        {
            return _commandsRepository.GetAllCommands();
        }

        [HttpGet("find")]
        public IEnumerable<CommandEntity> Get(string sourceName)
        {
            return _commandsRepository.FindBySourceNames(sourceName);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public CommandEntity Get(int id)
        {
            return _commandsRepository.GetCommandById(id);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] CommandEntity value)
        {
            _commandsRepository.SaveCommand(value);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] CommandEntity value)
        {
            value.CommandId = id;
            _commandsRepository.UpdateCommandEntity(value);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _commandsRepository.DeleteCommandEntity(id);
        }
    }
}
