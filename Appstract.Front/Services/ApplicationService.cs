using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appstract.Front.Entities;
using Appstract.Front.Repositories;
using MongoDB.Driver;

namespace Appstract.Front.Services
{
    public class ApplicationService
    {
        private readonly ApplicationRepository _repo;
        private readonly RpcChannel _channel;

        public ApplicationService(ApplicationRepository repo, RpcChannel channel)
        {
            _repo = repo;
            _channel = channel;
        }

        public async Task<dynamic> GetApplications()
        {
            return new 
            {
                Applications = await _repo.GetApplications(),
                PageNumbers = _repo.GetPagesLength()
            };
        }
        
        public Application CreateApplication(Application application)
        {
            return _repo.CreateApplication(application);
        }

        public void CreateModel(Application application)
        {
            
        }
    }
}