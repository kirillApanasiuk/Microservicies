using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _platformRepo;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(
            IPlatformRepo platformRepo,
            IMapper mapper,
            ICommandDataClient commandDataClient,
            IMessageBusClient messageBusClient
            )
        {
            _platformRepo = platformRepo;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            var models = _platformRepo.GetAllPlatforms();

            var dtos = _mapper.Map<IEnumerable<PlatformReadDto>>(models);

            return Ok(dtos);
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            var model = _platformRepo.GetPlatformById(id);

            if (model != null)
            {
                return Ok(_mapper.Map<PlatformReadDto>(model));
            }

            throw new ArgumentNullException(nameof(model));
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatPlatform(PlatformCreateDto createDto)
        {
            if (createDto == null)
            {
                throw new ArgumentNullException();
            }

            var model = _mapper.Map<Platform>(createDto);

            _platformRepo.CreatePlatform(model);
            _platformRepo.SaveChanges();


            var dto = _mapper.Map<PlatformReadDto>(model);

            try
            {

                await _commandDataClient.SendPlatformToCommand(dto);
                
            }
            catch (Exception ex)
            {

                Console.WriteLine($"--> Could not send synchronously {ex.Message}");
            }

            try
            {
                var publishDto = _mapper.Map<PlatformPublishedDto>(dto);

                publishDto.Event = "Platform_Published";
                _messageBusClient.PublishNewPlatform(publishDto);

                Console.WriteLine($"Message published to rabbit mq {publishDto}");
            }
            catch (Exception ex)
            {

                Console.WriteLine($"--> Could not send via rabbit{ex.Message}");
            }

            return CreatedAtRoute(nameof(GetPlatformById), new { Id = dto.Id }, dto);
        }
    }
}
