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
        private const    string                  DefaultName = "New Application";

        public HomeController(ILogger<HomeController> logger, MongoRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        [Route("application")]
        [HttpPost]
        public async Task<ActionResult<Application>> CreateApplication()
        {
            return await _repo.CreateApplication(new Application(DefaultName));
        }
        
        [Route("application")]
        [HttpGet]
        public async Task<ActionResult<Application>> GetApplication(string applicationId)
        {
            return await _repo.GetApplication(applicationId);
        }
        
        [Route("application/rename")]
        [HttpPost]
        public async Task<IActionResult> RenameApplication(RenameApplicationViewModel model)
        {
            await _repo.RenameApplication(model.ApplicationId, model.NewName);
            return Ok();
        }

        [Route("template")]
        [HttpPost]
        public async Task<ActionResult<Template>> CreateTemplate(string applicationId)
        {
            return await _repo.CreateTemplate(new Template(applicationId));
        }

        [Route("template")]
        [HttpPatch]
        public async Task<IActionResult> RenameTemplate(string templateId, string newName)
        {
            await _repo.RenameTemplate(templateId, newName);
            return Ok();
        }
    }
}
