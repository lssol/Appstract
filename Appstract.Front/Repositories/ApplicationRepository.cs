using System.Collections.Generic;
using System.Threading.Tasks;
using appcrawl.Options;
using Appstract.Front.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Appstract.Front.Repositories
{
    public class ApplicationRepository
    {
        private readonly MongoOptions _options;
        private readonly IMongoCollection<Application> _applicationCollection;
        private readonly IMongoCollection<Page> _pageCollection;

        public ApplicationRepository(IOptionsMonitor<MongoOptions> options)
        {
            _options = options.CurrentValue;
            var db = new MongoClient(_options.ConnectionString).GetDatabase(_options.Database);
            _applicationCollection = db.GetCollection<Application>(nameof(Application));
            _pageCollection = db.GetCollection<Page>(nameof(Page));
        }

        public Application CreateApplication(Application application)
        {
            _applicationCollection.InsertOne(application);
            return application;
        }

        public Task<List<Application>> GetApplications()
        {
            return _applicationCollection
                .Aggregate()
                .Lookup<Application, Page, Application>(_pageCollection, a => a.Id, p => p.ApplicationId, a => a.Pages)
                // .Lookup("Page", "Id", "ApplicationId", "Pages")
                // .As<Application>()
                .ToListAsync();
        }

        public async Task RemoveApplication(string idApplication)
        {
            await _applicationCollection.DeleteOneAsync(a => a.Id == idApplication);
        }

        public async Task UpdateApplication(Application application)
        {
            await _applicationCollection.ReplaceOneAsync(a => a.Id == application.Id, application);
        }
    }
}