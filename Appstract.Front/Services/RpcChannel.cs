using Grpc.Core;
using ProtoAppstract;

namespace Appstract.Front.Services
{
    public class RpcChannel
    {
        public readonly Robot.RobotClient RobotClient;
        public readonly Clustering.ClusteringClient ClusteringClient;
        private readonly Channel _channel;

        public RpcChannel()
        {
            const int port = 50051;
            _channel = new Channel($"127.0.0.1:{port}", ChannelCredentials.Insecure);
            RobotClient = new Robot.RobotClient(_channel);
            ClusteringClient = new Clustering.ClusteringClient(_channel);
        }

        public void Deconstruct()
        {
            _channel.ShutdownAsync().Wait();
        }
    }
}