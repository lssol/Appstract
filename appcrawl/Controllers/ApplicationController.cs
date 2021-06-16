﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using appcrawl.Entities;
using Microsoft.AspNetCore.Mvc;
using appcrawl.Models;
using appcrawl.Options;
using appcrawl.Repositories;
using Appstract.Types;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace appcrawl.Controllers
{
    [ApiController]
    [Route("/")]
    public class ApplicationController : Controller
    {
        private readonly ApplicationRepository        _repo;
        private readonly IMemoryCache                 _cache;
        private const    string                       DefaultNameApplication = "New Application";
        static           HttpClient                   _client                = new();

        public ApplicationController(ApplicationRepository repo, IMemoryCache cache)
        {
            _repo       = repo;
            _cache = cache;
        }

        [Route("application")]
        [HttpPost]
        public async Task<ActionResult<Application>> CreateApplication()
        {
            return await _repo.CreateApplication(new Application(DefaultNameApplication));
        }

        [Route("applications")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Application>>> GetApplications()
        {
            var res = await  _repo.GetApplications();
            return await res.ToListAsync();
        }
        
        [Route("application")]
        [HttpDelete]
        public async Task<IActionResult> RemoveApplication(RemoveApplicationModel model)
        {
            await _repo.RemoveApplication(model.ApplicationId);
            return Ok();
        }

        [Route("application")]
        [HttpGet]
        public async Task<ActionResult<Application>> GetApplication(string applicationId)
        {
            return _repo.GetApplication(applicationId);
        }
        
        [Route("application/rename")]
        [HttpPost]
        public async Task<IActionResult> RenameApplication(RenameApplicationModel model)
        {
            await _repo.RenameApplication(model.ApplicationId, model.NewName);
            return Ok();
        }
        
        [Route("application/model")]
        [HttpPost]
        public async Task<IActionResult> CreateModel(CreateModelViewModel model)
        {
            var appModel = Appstract.ModelCreation.createModel(model.Pages);
            var binaryModel = Appstract.ModelCreation.serializeModel(appModel);
            await _repo.UpdateModelApplication(model.ApplicationId, binaryModel);
            
            return Ok();
        }

        [Route("application/identify")]
        [HttpGet]
        public async Task<IActionResult> Identify(IdentifyPageModel m)
        {
            var app   = _repo.GetApplication(m.ApplicationId);

            var model = _cache.GetOrCreate(m.ApplicationId, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(3);
                return Appstract.ModelCreation.createModel(app.Templates.Select(t => t.Html));
            });
            
            var identification = Appstract.ModelCreation.identifyPage(model, m.Page);
            
            return Ok(identification);
        }
    }
}
