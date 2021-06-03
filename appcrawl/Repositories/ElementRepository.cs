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

namespace appcrawl.Repositories
{
    public class ElementRepository
    {
        private readonly MongoOptions _options;
        private readonly IMongoCollection<Application> _applicationCollection;
        private readonly IMongoCollection<Template> _templateCollection;
        private readonly IMongoCollection<Element> _elementCollection;

        public ElementRepository(IOptionsMonitor<MongoOptions> options)
        {
            _options = options.CurrentValue;
            _elementCollection = new MongoClient(_options.ConnectionString)
                .GetDatabase(_options.Database)
                .GetCollection<Element>(nameof(Element));
        }

        public async Task RenameElement(string idTemplate, string newName)
        {
            var update = Builders<Element>.Update.Set(a => a.Name, newName);
            await _elementCollection.UpdateOneAsync(a => a.Id == idTemplate, update);
        }

        public void RemoveElement(string id)
        {
            _elementCollection.DeleteOne(t => t.Id == id);
        }
        
        public async Task<Element> CreateElement(Element element)
        {
            await _elementCollection.InsertOneAsync(element);
            return element;
        }
    }
}