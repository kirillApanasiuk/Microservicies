using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandsService.SyncDataServices.Grpc
{
    public class PlatformDataClient : IPlatformDataClient, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public PlatformDataClient(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }

        public void Dispose()
        {

        }

        public IEnumerable<Platform> ReturnAllPlatforms()
        {
            var grpcPlatformAddress = _configuration["GrpcPlatrorm"];
            System.Console.WriteLine($"--> Calling GRPC Service {grpcPlatformAddress}");

            var channel = GrpcChannel.ForAddress(grpcPlatformAddress);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);

            var request = new GetAllRequest();

            try
            {
                var reply = client.GetAllPlatforms(request);
                return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"--> Couldnot call Grpc server {ex.Message}");
                return null;
            }
        }
    }
}