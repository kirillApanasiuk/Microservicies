using AutoMapper;
using CommandsService.Data;
using CommandsService.DTOs;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepo _commandRepo;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepo commandRepo, IMapper mapper)
        {
            _commandRepo = commandRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
        {
            Console.WriteLine($"--> Hit {nameof(GetCommandsForPlatform)} ");

            if (!_commandRepo.PlatformExists(platformId))
            {
                return NotFound("Not implemented yet");
            }

            var commands = _commandRepo.GetCommadsForPlatform(platformId);


            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }

        [HttpGet("{commandId}", Name = $"{nameof(GetCommandForPlatform)}")]

        public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId,int commandId)
        {
            Console.WriteLine($"{platformId} - platformId, {commandId} - commandId");

            Console.WriteLine($"Not implemented {nameof(GetCommandForPlatform)}");

            if (! _commandRepo.PlatformExists(platformId))
            {
                return NotFound();
            }

            var command = _commandRepo.GetCommand(platformId,commandId);

            if (command == null)
            {
                return NotFound();
            }

            var dto = _mapper.Map<CommandReadDto>(command);
            return dto;
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CreateCommandDto commandCreateDto)
        {

            Console.WriteLine($"{platformId} - platformId");

            Console.WriteLine($"Not implemented {nameof(CreateCommandForPlatform)}");

            if (!_commandRepo.PlatformExists(platformId))
            {
                return NotFound();
            }

            var command = _mapper.Map<Command>(commandCreateDto);

            _commandRepo.CreateCommand(platformId,command);

            _commandRepo.SaveChanges();

            var dto = _mapper.Map<CommandReadDto>(command);

            return CreatedAtRoute(nameof(GetCommandForPlatform),new { platformId = platformId, commandId  = command.Id}, dto);
        }


    }
}
