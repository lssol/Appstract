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

        public Task<List<Application>> GetApplications()
        {
            return _repo.GetApplications();
        }
        public Application CreateApplication(Application application)
        {
            return _repo.CreateApplication(application);
        }
    }
}