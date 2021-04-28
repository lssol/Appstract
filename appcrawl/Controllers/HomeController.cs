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
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MongoRepository _repo;

        public HomeController(ILogger<HomeController> logger, MongoRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        [Route("application")]
        [HttpPost]
        public ActionResult<Application> CreateApplication()
        {
            return new Application();
        }
        
        [Route("application")]
        [HttpPatch]
        public ActionResult<Application> RenameApplication(string idApplication, string newName)
        {
            return new Application();
        }

        [Route("template")]
        [HttpPost]
        public ActionResult<Template> CreateTemplate(string idApplication)
        {
            return new Template();
        }

        [Route("template")]
        [HttpPatch]
        public ActionResult<Template> RenameTemplate(string idTemplate)
        {
            return new Template();
        }
    }
}
