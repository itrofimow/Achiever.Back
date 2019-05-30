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

        //ImageInfo DownscaleImage(string id);
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
            /*var data = imageStream.ToArray();

            var loadedImage = DecodeImage(data);

            return new ImageInfo
            {
                ImagePath = await _fileService.SaveFile(data),
                Width = loadedImage.Width,
                Height = loadedImage.Height
            };*/

            var filePath = await _fileService.GetSaveFilename();

            await _fileService.SaveFile(imageStream);
            imageStream.Position = 0;

            using (var fs = _fileService.GetSaveFileStream($"{filePath}_downscaled"))
            {
                return DownscaleImage(filePath, imageStream, fs);
            }
        }

        public ImageInfo DownscaleImage(string filePath, Stream dataStream, Stream destination)
        {
            const float size = 600;
            
            var originalImage = SKBitmap.Decode(dataStream);

            var scale = Math.Min(1,
                Math.Max(size / originalImage.Width, size / originalImage.Width));
            
            var resultName = $"{filePath}_downscaled";
            int resultWidth;
            int resultHeight;
            
            using (var resultImage = originalImage.Resize(new SKImageInfo(
                (int) (originalImage.Width * scale),
                (int) (originalImage.Height * scale)), SKFilterQuality.Medium))
            {
                using (var image = SKImage.FromBitmap(resultImage))
                {
                    resultWidth = image.Width;
                    resultHeight = image.Height;
                    
                    using (var data = image.Encode(SKEncodedImageFormat.Webp, 100))
                    {
                        data.SaveTo(destination);
                    }
                }
            }

            return new ImageInfo
            {
                ImagePath = $"uploads/{resultName}",
                Width = resultWidth,
                Height = resultHeight
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