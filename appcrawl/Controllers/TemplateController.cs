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
    [Route("/template")]
    public class TemplateController : Controller
    {
        private readonly TemplateRepository _repo;
        private readonly RobotOptions _robotOptions;
        private const    string                  DefaultNameTemplate = "New Template";
        static readonly HttpClient Client = new();

        public TemplateController(TemplateRepository repo, IOptionsMonitor<RobotOptions> robotOptions)
        {
            _repo = repo;
            _robotOptions = robotOptions.CurrentValue;
        }

        [Route("remove")]
        [HttpDelete]
        public async Task<IActionResult> RemoveTemplate(RemoveTemplateModel model)
        {
            await _repo.RemoveTemplate(model.TemplateId);
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<Template>> CreateTemplate(CreateTemplateModel model)
        {
            return await _repo.CreateTemplate(new Template(model.ApplicationId, DefaultNameTemplate));
        }

        [Route("rename")]
        [HttpPost]
        public async Task<IActionResult> RenameTemplate(RenameTemplateModel model)
        {
            await _repo.RenameTemplate(model.TemplateId, model.NewName);
            return Ok();
        }
        
        [Route("url")]
        [HttpPost]
        public async Task<IActionResult> SetUrlTemplate(SetUrlTemplateModel model)
        {
            var url = QueryHelpers.AddQueryString(_robotOptions.Url + "/urltohtml", "url", model.Url);
            var res = await Client.GetAsync(url);
            res.EnsureSuccessStatusCode();
            var body = await res.Content.ReadFromJsonAsync<RobotCall>();
            await _repo.SetUrlTemplate(model.TemplateId, model.Url, body.Html);
            
            return Ok(new {Html = body.Html});
        }
    }
}
