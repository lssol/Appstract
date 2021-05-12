using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using appcrawl.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using appcrawl.Models;
using appcrawl.Repositories;

namespace appcrawl.Controllers
{
    [ApiController]
    [Route("/")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MongoRepository         _repo;
        private const    string                  DefaultNameApplication = "New Application";
        private const    string                  DefaultNameTemplate = "New Template";

        public HomeController(ILogger<HomeController> logger, MongoRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        [Route("application")]
        [HttpPost]
        public async Task<ActionResult<Application>> CreateApplication()
        {
            return await _repo.CreateApplication(new Application(DefaultNameApplication));
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
        
        [Route("template/remove")]
        [HttpDelete]
        public async Task<IActionResult> RemoveTemplate(RemoveTemplateModel model)
        {
            await _repo.RemoveTemplate(model.TemplateId);
            return Ok();
        }

        [Route("template")]
        [HttpPost]
        public async Task<ActionResult<Template>> CreateTemplate(CreateTemplateModel model)
        {
            return await _repo.CreateTemplate(new Template(model.ApplicationId, DefaultNameTemplate));
        }

        [Route("Applications")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Application>>> GetApplications()
        {
            return Ok(await _repo.GetApplications());
        }
        
        [Route("template/rename")]
        [HttpPost]
        public async Task<IActionResult> RenameTemplate(RenameTemplateModel model)
        {
            await _repo.RenameTemplate(model.TemplateId, model.NewName);
            return Ok();
        }
        
        [Route("template/url")]
        [HttpPost]
        public async Task<IActionResult> SetUrlTemplate(SetUrlTemplateModel model)
        {
            await _repo.SetUrlTemplate(model.TemplateId, model.Url);
            return Ok();
        }
    }
}
