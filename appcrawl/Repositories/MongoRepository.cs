using System;
using appcrawl.Options;
using Microsoft.Extensions.Options;

namespace appcrawl.Repositories
{
    public class MongoRepository
    {
        private readonly MongoOptions _options;

        public MongoRepository(IOptionsMonitor<MongoOptions> options)
        {
            _options = options.CurrentValue;
        }
    }
}