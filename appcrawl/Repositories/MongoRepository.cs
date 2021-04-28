using System;
using System.Threading.Tasks;
using appcrawl.Entities;
using appcrawl.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver;

namespace appcrawl.Repositories
{
    public class MongoRepository
    {
        private readonly MongoOptions _options;
        private readonly IMongoCollection<Application> _applicationCollection;
        private readonly IMongoCollection<Template> _templateCollection;

        public MongoRepository(IOptionsMonitor<MongoOptions> options)
        {
            _options = options.CurrentValue;
            var db = new MongoClient(_options.ConnectionString).GetDatabase(_options.Database);
            _applicationCollection = db.GetCollection<Application>(nameof(Application));
            _templateCollection = db.GetCollection<Template>(nameof(Template));
        }

        private async Task<Application> CreateApplication(Application application)
        {
            await _applicationCollection.InsertOneAsync(application);
            return application;
        }
        
        private async Task<Template> CreateTemplate(Template template)
        {
            await _templateCollection.InsertOneAsync(template);
            return template;
        }

        private async Task RenameApplication(string idApplication, string newName)
        {
            var update = Builders<Application>.Update.Set(a => a.Name, newName);
            await _applicationCollection.UpdateOneAsync(a => a.Id == idApplication, update);
        }
    }
}