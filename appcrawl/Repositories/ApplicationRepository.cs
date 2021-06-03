using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using appcrawl.Controllers;
using appcrawl.Entities;
using appcrawl.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
// ReSharper disable ReplaceWithSingleCallToFirstOrDefault

namespace appcrawl.Repositories
{
    public class ApplicationRepository
    {
        private readonly TemplateRepository _templateRepository;
        private readonly MongoOptions _options;
        private readonly IMongoCollection<Application> _applicationCollection;

        public ApplicationRepository(IOptionsMonitor<MongoOptions> options, TemplateRepository templateRepository)
        {
            _templateRepository = templateRepository;
            _options = options.CurrentValue;
            
            _applicationCollection = new MongoClient(_options.ConnectionString)
                .GetDatabase(_options.Database)
                .GetCollection<Application>(nameof(Application));
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
            var applications = _applicationCollection
                .AsQueryable()
                .Where(a => a.Id == id)
                .FirstOrDefault();

            var templates = _templateRepository.GetTemplates(id);
                
            applications.Templates = templates;

            return applications;
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
        
        public async Task UpdateModelApplication(string idApplication, byte[] model)
        {
            var update = Builders<Application>.Update.Set(a => a.Model, model);
            await _applicationCollection.UpdateOneAsync(a => a.Id == idApplication, update);
        }
    }
}