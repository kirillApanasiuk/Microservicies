using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.DTOs;
using CommandsService.Models;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IMapper _mapper;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public EventProcessor(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
        {
            _mapper = mapper;
            _serviceScopeFactory = serviceScopeFactory;
        }
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);
            switch (eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(message);
                    break;
                default:
                    break;
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        {

            System.Console.WriteLine("--> Determining Event");

            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

            switch (eventType.Event)
            {
                case "Platform_Published":
                    System.Console.WriteLine("Platform published event detected");
                    return EventType.PlatformPublished;
                default:
                    System.Console.WriteLine("--> Could not determine event type");
                    return EventType.Undefined;
            }
        }

        private void AddPlatform(string platformPublishedMessage)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
                var platforPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);
                System.Console.WriteLine($"Json representation {platformPublishedMessage}\n ");
                System.Console.WriteLine($"Standard representation {platforPublishedDto}");
                try
                {
                    var platform = _mapper.Map<Platform>(platforPublishedDto);
                    if (!repo.ExternalPlatformExist(platform.ExternalId))
                    {
                        repo.CreatePlatform(platform);
                        repo.SaveChanges();
                    }
                    else
                    {
                        System.Console.WriteLine("--> Platform already exists...");
                    }
                }
                catch (System.Exception ex)
                {

                    System.Console.WriteLine($"--> Could not add Platform to Db {ex.Message}");
                }
            }
        }
    }

    enum EventType
    {
        PlatformPublished,
        Undefined
    }
}