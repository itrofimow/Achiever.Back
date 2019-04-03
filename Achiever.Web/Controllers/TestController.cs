using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Achiever.Core.DbInterfaces;
using Achiever.Core.Feed;
using Achiever.Core.Models.Feed;
using Achiever.Core.Models.Test;
using Microsoft.AspNetCore.Mvc;

namespace Achiever.Web.Controllers
{
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ITestRepository _testRepository;
        private readonly IFeedService _feedService;

        public TestController(
            ITestRepository testRepository,
            IFeedService feedService)
        {
            _testRepository = testRepository;
            _feedService = feedService;
        }

        [HttpPost]
        [Route("A")]
        public Task InsertA(ModelA model)
        {
            return _testRepository.InsertA(model);
        }

        [HttpPost]
        [Route("B")]
        public Task InsertB(ModelB model)
        {
            return _testRepository.InsertB(model);
        }
        
        [HttpGet]
        public Task<ModelA> Get(string id)
        {
            return _testRepository.Get(id);
        }

        /*[HttpGet]
        [Route("test")]
        public Task<List<FeedEntryResponse>> Test()
        {
            return _feedService.GetFeedPage("5c458c61c157a824fc91c644", DateTime.Now, 0, 20);
        }*/
        
    }
}