using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using appcrawl.Controllers;
using appcrawl.Entities;
using appcrawl.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace appcrawl.Repositories
{
    public class TemplateRepository
    {
        private readonly MongoOptions _options;
        private readonly IMongoCollection<Template> _templateCollection;

        public TemplateRepository(IOptionsMonitor<MongoOptions> options)
        {
            _options = options.CurrentValue;
            _templateCollection = new MongoClient(_options.ConnectionString)
                .GetDatabase(_options.Database)
                .GetCollection<Template>(nameof(Template));
        }

        public async Task<Template> CreateTemplate(Template template)
        {
            await _templateCollection.InsertOneAsync(template);
            return template;
        }

        public IEnumerable<Template> GetTemplates(string applicationId)
        {
            return _templateCollection.Aggregate()
                .Match(x => x.ApplicationId == applicationId)
                .AppendStage<BsonDocument>("{$addFields: {id: {$toString:'$_id'}}}")
                .Lookup(nameof(Element), "id", "TemplateId", "Elements")
                .Project("{id: 0}")
                .As<Template>()
                .ToList();
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

        public Template GetTemplate(string templateId)
        {
            return _templateCollection.AsQueryable().Where(t => t.Id == templateId).FirstOrDefault();
        }
    }
}