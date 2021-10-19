using System;
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
using FSharpPlus.Control;
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
        
        [Route("application/host")]
        [HttpPut]
        public async Task<IActionResult> SetHost(SetHostApplicationModel model)
        {
            await _repo.UpdateHost(model.ApplicationId, model.Host);
            return Ok();
        }
        
        [Route("application/model")]
        [HttpPost]
        public async Task<IActionResult> CreateModel(CreateModelViewModel model)
        {
            var appModel    = Appstract.ModelCreation.createModel(model.Templates.Select(t => new Tuple<string, string>(t.TemplateId, t.Content)));
            var binaryModel = Appstract.ModelCreation.serializeModel(appModel);
            await _repo.UpdateModelApplication(model.ApplicationId, binaryModel);
            _cache.Remove(model.ApplicationId);
            
            return Ok();
        }

        [Route("application/identify")]
        [HttpGet]
        public async Task<IActionResult> Identify(IdentifyPageModel m)
        {
            var model = _cache.GetOrCreate(m.Host, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(3);
                
                var app  = _repo.GetApplicationFromHost(m.Host);
                if (app.Model == null)
                    throw new Exception("No Associated Model");
                return Appstract.ModelCreation.unserializeModel(app.Model);
            });
            
            var identification = Appstract.ModelCreation.identifyPage(model, m.Page);
            
            return Ok(identification);
        }
    }
}
