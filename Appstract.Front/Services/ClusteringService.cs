using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Appstract.Front.Entities;
using ProtoAppstract;

namespace Appstract.Front.Services
{
    public class ClusteringService
    {
        public bool _synchronizing;
        public List<string> _messages = new();
        public event Action OnChange;
        private readonly RpcChannel _rpcChannel;
        
        public ClusteringService(RpcChannel rpcChannel)
        {
            _rpcChannel = rpcChannel;
        }

        public async Task StartClustering()
        {
            _messages.Clear();
            OnChange?.Invoke();
            var result = _rpcChannel.ClusteringClient.StartClustering(new Empty());
            _synchronizing = true;
            var stream = result.ResponseStream;
            while (await stream.MoveNext(CancellationToken.None))
            {
                var message = stream.Current;
                _messages.Add(message.Message);
                OnChange?.Invoke();
            }
            _synchronizing = false;
        }
    }
}