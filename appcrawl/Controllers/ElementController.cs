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
    [Route("/element")]
    public class ElementController : Controller
    {
        private readonly ElementRepository _repo;
        private readonly RobotOptions _robotOptions;
        private const string DefaultNameElement = "New Element";
        static HttpClient _client = new HttpClient();

        public ElementController(ElementRepository repo, IOptionsMonitor<RobotOptions> robotOptions)
        {
            _repo = repo;
            _robotOptions = robotOptions.CurrentValue;
        }

        [Route("remove")]
        [HttpDelete]
        public async Task<IActionResult> RemoveElement(RemoveElementModel model)
        {
            _repo.RemoveElement(model.ElementId);
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<Element>> CreateElement(CreateElementModel model)
        {
            return await _repo.CreateElement(new Element(model.ApplicationId, model.TemplateId , DefaultNameElement));
        }

        [Route("rename")]
        [HttpPost]
        public async Task<IActionResult> RenameElement(RenameElementModel model)
        {
            await _repo.RenameElement(model.ElementId, model.Name);
            return Ok();
        }
        
        [Route("signature")]
        [HttpPost]
        public async Task<IActionResult> UpdateSignature(UpdateSignatureElementModel model)
        {
            await _repo.UpdateSignature(model.ElementId, model.Signature);
            return Ok();
        }
    }
}