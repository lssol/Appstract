using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using appcrawl.Entities;
using appcrawl.Options;
using Microsoft.AspNetCore.Mvc;
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

        public Task<IAsyncCursor<Application>> GetApplications()
        {
            return _applicationCollection
                .FindAsync(Builders<Application>.Filter.Empty);
        }
        
        public Application GetApplication(string id)
        {
            var applications = _applicationCollection.AsQueryable()
                .Where(a => a.Id == id).FirstOrDefault();
            var templates = _templateCollection.AsQueryable()
                .Where(t => t.ApplicationId == id).ToList();
            applications.Templates = templates;

            return applications;
        }
        
        public async Task<Template> CreateTemplate(Template template)
        {
            await _templateCollection.InsertOneAsync(template);
            return template;
        }
        
        public async Task RemoveApplication(string idApplication)
        {
            await _applicationCollection.DeleteOneAsync(a => a.Id == idApplication);
        }

        public async Task RenameApplication(string idApplication, string newName)
        {
            var update = Builders<Application>.Update.Set(a => a.Name, newName);
            await _applicationCollection.UpdateOneAsync(a => a.Id == idApplication, update);
        }
        
        public async Task SetUrlTemplate(string templateId, string url, string html)
        {
            var update = Builders<Template>.Update
                .Set(a => a.Url, url)
                .Set(a => a.Html, html);
            await _templateCollection.UpdateOneAsync(a => a.Id == templateId, update);
        }
        
        public async Task RenameTemplate(string idTemplate, string newName)
        {
            var update = Builders<Template>.Update.Set(a => a.Name, newName);
            await _templateCollection.UpdateOneAsync(a => a.Id == idTemplate, update);
        }

        public async Task RemoveTemplate(string id)
        {
            _templateCollection.DeleteOne(t => t.Id == id);
        }

        public async Task<IEnumerable<Application>> GetApplications()
        { 
            return await _applicationCollection.Find(_ => true).ToListAsync();
        }
    }
}