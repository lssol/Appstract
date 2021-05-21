using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using appcrawl.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using appcrawl.Models;
using appcrawl.Options;
using appcrawl.Repositories;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace appcrawl.Controllers
{
    [ApiController]
    [Route("/")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MongoRepository         _repo;
        private readonly RobotOptions _robotOptions;
        private const    string                  DefaultNameApplication = "New Application";
        private const    string                  DefaultNameTemplate = "New Template";
        static HttpClient _client = new HttpClient();

        public HomeController(ILogger<HomeController> logger, MongoRepository repo, IOptionsMonitor<RobotOptions> robotOptions)
        {
            _logger = logger;
            _repo = repo;
            _robotOptions = robotOptions.CurrentValue;
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
            var url = QueryHelpers.AddQueryString(_robotOptions.Url + "/urltohtml", "url", model.Url);
            var res = await _client.GetAsync(url);
            res.EnsureSuccessStatusCode();
            var body = await res.Content.ReadFromJsonAsync<RobotCall>();
            await _repo.SetUrlTemplate(model.TemplateId, model.Url, body.Html);
            return Ok(new {Html = body.Html});
        }
    }
}
