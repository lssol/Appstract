using Grpc.Core;

namespace Appstract.Front.Services
{
    public class RpcChannel
    {
        public readonly Robot.Robot.RobotClient Client;
        private readonly Channel _channel;

        public RpcChannel()
        {
            const int port = 50051;
            _channel = new Channel($"127.0.0.1:{port}", ChannelCredentials.Insecure);
            Client = new Robot.Robot.RobotClient(_channel);
        }

        public void Deconstruct()
        {
            _channel.ShutdownAsync().Wait();
        }
    }
}