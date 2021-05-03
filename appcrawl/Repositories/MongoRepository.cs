using System;
using System.Threading.Tasks;
using appcrawl.Entities;
using appcrawl.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

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

        public async Task<Application> CreateApplication(Application application)
        {
            await _applicationCollection.InsertOneAsync(application);
            return application;
        }
        
        public Task<Application> GetApplication(string id)
        {
            return _applicationCollection.AsQueryable()
                .Where(a => a.Id == id).FirstOrDefaultAsync();
        }
        
        public async Task<Template> CreateTemplate(Template template)
        {
            await _templateCollection.InsertOneAsync(template);
            return template;
        }

        public async Task RenameApplication(string idApplication, string newName)
        {
            var update = Builders<Application>.Update.Set(a => a.Name, newName);
            await _applicationCollection.UpdateOneAsync(a => a.Id == idApplication, update);
        }
        
        public async Task RenameTemplate(string idTemplate, string newName)
        {
            var update = Builders<Template>.Update.Set(a => a.Name, newName);
            await _templateCollection.UpdateOneAsync(a => a.Id == idTemplate, update);
        }
    }
}