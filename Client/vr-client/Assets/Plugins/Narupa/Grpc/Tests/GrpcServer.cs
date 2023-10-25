using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Narupa.Core.Async;
using Narupa.Grpc.Tests.Trajectory;

namespace Narupa.Grpc.Tests.Multiplayer
{
    /// <summary>
    /// Generic gRPC server for testing purposes.
    /// </summary>
    internal class GrpcServer : IAsyncClosable
    {
        private Server server;

        private LatencySimulator latency;

        public int StreamLatency
        {
            get => latency.ServerStreamLatency;
            set => latency.ServerStreamLatency = value;
        }
        
        public int ReplyLatency
        {
            get => latency.ServerReplyLatency;
            set => latency.ServerReplyLatency = value;
        }

        public GrpcServer(params IBindableService[] services) : this(
            services.Select(s => s.BindService()).ToArray())
        {
        }

        public GrpcServer(params ServerServiceDefinition[] services)
        {
            server = new Server
            {
                Ports =
                {
                    new ServerPort("localhost", 0, ServerCredentials.Insecure)
                }
            };
            latency = new LatencySimulator();
            foreach (var service in services)
                server.Services.Add(service.Intercept(latency));
            server.Start();
        }

        public int Port => server.Ports.First().BoundPort;

        public async Task CloseAsync()
        {
            if (server == null)
                return;
            await server.KillAsync();
            server = null;
        }

        public static (GrpcServer server, GrpcConnection connection) CreateServerAndConnection(params IBindableService[] services)
        {
            var server = new GrpcServer(services);
            return (server, new GrpcConnection("localhost", server.Port));
        }
    }
}