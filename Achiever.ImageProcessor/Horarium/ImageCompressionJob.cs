using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Achiever.Core;
using Achiever.Core.DbInterfaces;
using Horarium.Interfaces;
using Microsoft.Extensions.Logging;
using SkiaSharp;

namespace Achiever.ImageProcessor.Horarium
{
    public class ImageCompressionJob : IJobRecurrent
    {
        private readonly ILogger<ImageCompressionJob> _logger;
        private readonly IFilesRepository _filesRepository;
        private readonly IImageProcessingService _imageProcessingService;

        public ImageCompressionJob(
            ILogger<ImageCompressionJob> logger,
            IFilesRepository filesRepository,
            IImageProcessingService imageProcessingService)
        {
            _logger = logger;
            _filesRepository = filesRepository;
            _imageProcessingService = imageProcessingService;
        }
        
        public async Task Execute()
        {
            return;
            var filesToProcessTasks = Enumerable.Range(0, 8)
                .Select(x => _filesRepository.StartImageCompression())
                .ToList();

            var filesToProcess = await Task.WhenAll(filesToProcessTasks);
        }

        public async Task<string> CompressImage(string originalPath)
        {
            //var imageInfo = _imageProcessingService.DownscaleImage(originalPath);

            return null;
        }
    }
}