using Grpc.Core;
using ProtoAppstract;

namespace Appstract.Front.Services
{
    public class RpcChannel
    {
        public readonly Robot.RobotClient RobotClient;
        public readonly Clustering.ClusteringClient ClusteringClient;
        private readonly Channel _channelRobot;
        private readonly Channel _channelClustering;

        public RpcChannel()
        {
            const int portRobot = 50076;
            const int portClustering = 50052;
            _channelRobot = new Channel($"127.0.0.1:{portRobot}", ChannelCredentials.Insecure);
            _channelClustering = new Channel($"127.0.0.1:{portClustering}", ChannelCredentials.Insecure);
            RobotClient = new Robot.RobotClient(_channelRobot);
            ClusteringClient = new Clustering.ClusteringClient(_channelClustering);
        }

        public void Deconstruct()
        {
            var a = _channelRobot.ShutdownAsync();
            _channelClustering.ShutdownAsync().Wait();
            a.Wait();
        }
    }
}