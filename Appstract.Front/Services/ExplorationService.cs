using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Appstract.Front.Entities;
using Appstract.Front.Repositories;

namespace Appstract.Front.Services
{
    public class ExplorationService
    {
        private readonly RpcChannel _rpcChannel;
        private readonly ApplicationRepository _applicationRepository;
        public event Action OnChange;

        public ExplorationService(RpcChannel rpcChannel, ApplicationRepository applicationRepository)
        {
            _rpcChannel = rpcChannel;
            _applicationRepository = applicationRepository;
        }

        public ConcurrentDictionary<string, int> exploration { get; } = new();
        
        public async Task Explore(Application application)
        {
            var explorationCanStart = exploration.TryAdd(application.Id, 0);
            if (!explorationCanStart)
            {
                Console.WriteLine($"An exploration is already running for {application.Domain}");
            }

            var stream = _rpcChannel.Client.Explore(new ExploreRequest {Domain = application.Domain}).ResponseStream;
            while (await stream.MoveNext(CancellationToken.None))
            {
                var res = stream.Current;
                if (res.Error)
                {
                    Console.WriteLine($"Error occured when exploring {res.Url}");
                    continue;
                }

                Console.WriteLine($"Explored {res.Url}");
                await _applicationRepository.CreatePage(new Page
                {
                    Content = res.Content,
                    Domain = res.Domain,
                    Origin = res.Origin,
                    Url = res.Url,
                    ApplicationId = application.Id,
                    NbLinks = res.NbLinks,
                    NbNodes = res.NbNodes
                });
                exploration.AddOrUpdate(application.Id, 1, (_,v) => v + 1);
                OnChange?.Invoke();
            }

            exploration.TryRemove(application.Id, out var value);
            Console.WriteLine($"Finished exploration of {application.Domain}: {value} pages explored.");
        }
    }
}