using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Achiever.Core;
using Achiever.Core.DbInterfaces;
using Achiever.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Achiever.Web.Controllers
{
    [Route("api/[controller]")]
    public class AchievementController : ControllerBase
    {
        private readonly IAcquiredAchievementRepository _acquiredAchievementRepository;
        private readonly IAchievementService _achievementService;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly IAchievementCategoryService _achievementCategoryService;

        public AchievementController(
            IAcquiredAchievementRepository acquiredAchievementRepository,
            IAchievementService achievementService,
            IImageProcessingService imageProcessingService,
            IAchievementCategoryService achievementCategoryService)
        {
            _acquiredAchievementRepository = acquiredAchievementRepository;
            _achievementService = achievementService;
            _imageProcessingService = imageProcessingService;
            _achievementCategoryService = achievementCategoryService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task Create([FromForm] CreateAchievementRequest request)
        {
            var achievement = new Achievement
            {
                Title = request.Title,
                Description = request.Description
            };

            achievement.BackgroundImage = await Process(request.BackgroundImage);
            achievement.FrontImage = await Process(request.FrontImage);
            //achievement.BigImage = await Process(request.BigImage);

            await _achievementService.Create(achievement);
        }

        private async Task<ImageInfo> Process(IFormFile image)
        {
            using (var ms = new MemoryStream())
            {
                await image.CopyToAsync(ms);
                ms.Position = 0;

                return await _imageProcessingService.SaveImage(ms);
            }
        }

        [HttpGet]
        [Route("all")]
        public async Task<GetAllAchievementsResponse> GetAll()
        {
            var data = await _achievementService.GetAll();

            return new GetAllAchievementsResponse
            {
                Achievements = data
            };
        }

        [HttpGet]
        [Route("id/{id}")]
        public Task<Achievement> GetById(string id)
        {
            return _achievementService.GetById(id);
        }

        [HttpGet]
        [Route("categories")]
        public async Task<GetAllAchievementCategoriesResponse> GetAllCategories()
        {
            var data = await _achievementCategoryService.GetAll();

            return new GetAllAchievementCategoriesResponse
            {
                Categories = data
            };
        }

        [HttpGet]
        [Route("categories/{id}")]
        public async Task<GetAllAchievementsResponse> GetAllByCategory(string id)
        {
            var data = await _achievementService.GetAllByCategory(id);

            return new GetAllAchievementsResponse
            {
                Achievements = data
            };
        }

        [HttpPost]
        [Route("categories")]
        public Task CreateCategory([FromBody] AchievementCategory category)
        {
            return _achievementCategoryService.Create(category);
        }
    }

    public class GetAllAchievementCategoriesResponse
    {
        public List<AchievementCategory> Categories { get; set; }
    }

    public class GetAllAchievementsResponse
    {
        public List<Achievement> Achievements { get; set; }
    }

    public class CreateAchievementRequest
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public IFormFile BackgroundImage { get; set; }

        public IFormFile FrontImage { get; set; }

        public IFormFile BigImage { get; set; }
    }
}