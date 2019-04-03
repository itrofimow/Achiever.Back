using System;
using System.IO;
using System.Threading.Tasks;
using Achiever.Core.Models;
using SkiaSharp;

namespace Achiever.Core
{
    public interface IImageProcessingService
    {
        Task<ImageInfo> SaveImage(MemoryStream imageStream);
    }
    
    public class ImageProcessingService : IImageProcessingService
    {
        private readonly IFileService _fileService;

        public ImageProcessingService(IFileService fileService)
        {
            _fileService = fileService;
        }
        
        public async Task<ImageInfo> SaveImage(MemoryStream imageStream)
        {
            var data = imageStream.ToArray();

            var loadedImage = DecodeImage(data);

            return new ImageInfo
            {
                ImagePath = await _fileService.SaveFile(data),
                Width = loadedImage.Width,
                Height = loadedImage.Height
            };
        }

        private SKBitmap DecodeImage(byte[] data)
        {
            var loadedImage = SKBitmap.Decode(data);
            if (loadedImage == null)
                throw new Exception("fucked up image");

            return loadedImage;
        }
    }
}