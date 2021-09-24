using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Threading.Tasks;
using appcrawl.Options;
using Appstract.Front.Entities;
using Appstract.Front.Utils;
using FSharpPlus;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Task = System.Threading.Tasks.Task;

namespace Appstract.Front.Repositories
{
    public class ApplicationRepository
    {
        private readonly MongoOptions _options;
        private readonly IMongoCollection<Application> _applicationCollection;
        private readonly IMongoCollection<Page> _pageCollection;

        private readonly List<Expression<Func<Page, object>>> pageIndices = new()
        {
            p => p.ApplicationId
        };

        public ApplicationRepository(IOptionsMonitor<MongoOptions> options)
        {
            _options = options.CurrentValue;
            var db = new MongoClient(_options.ConnectionString).GetDatabase(_options.Database);
            _applicationCollection = db.GetCollection<Application>(nameof(Application));
            _pageCollection = db.GetCollection<Page>(nameof(Page));
            CreateIndices();
        }

        public void CreateIndices()
        {
            pageIndices.ForEach(i =>
            {
                var indexKeysDefinition = Builders<Page>.IndexKeys.Ascending(i);
                _pageCollection.Indexes.CreateOne(new CreateIndexModel<Page>(indexKeysDefinition));
            });
        }

        public Application CreateApplication(Application application)
        {
            _applicationCollection.InsertOne(application);
            return application;
        }

        public Task<List<Application>> GetApplications()
        {
            return _applicationCollection.AsQueryable().ToListAsync();
        }

        public Dictionary<string, int> GetPagesLength()
        {
            return _pageCollection.AsQueryable()
                .GroupBy(p => p.ApplicationId)
                .Select(p => new Tuple<string, int>(p.Key, p.Count())) // Needed because of limitation of Linq2MQL
                .ToDictionary(p => p.Item1, p => p.Item2);
        }

        public List<Page> GetPages(string applicationId)
        {
            return _pageCollection.AsQueryable()
                .Where(p => p.ApplicationId == applicationId)
                .ToList();
        }

        public async Task RemoveApplication(string idApplication)
        {
            await _applicationCollection.DeleteOneAsync(a => a.Id == idApplication);
        }

        public async Task UpdateApplication(Application application)
        {
            await _applicationCollection.ReplaceOneAsync(a => a.Id == application.Id, application);
        }

        public async Task<Page> CreatePage(Page page)
        {
            page.PageHash = Hashing.GetHashString(page.Content);
            await _pageCollection.InsertOneAsync(page);
            return page;
        }
    }
}