using System.IO;
using System.Threading.Tasks;
using Achiever.Core.DbInterfaces;

namespace Achiever.Core
{
    public interface IFileService
    {
        Task<string> GetSaveFilename();

        FileStream GetSaveFileStream(string filename);

        Task<string> SaveFile(Stream file);

        Task<string> SaveFile(byte[] file);
    }
    
    public class FileService : IFileService
    {
        private const string UploadsDirectory = "uploads";
        
        private readonly string _webRoot;
        private readonly IFilesRepository _filesRepository;

        public FileService(string webRoot, IFilesRepository filesRepository)
        {
            _webRoot = Path.Combine(webRoot, "images", UploadsDirectory);
            Directory.CreateDirectory(_webRoot);

            _filesRepository = filesRepository;
        }

        public async Task<string> GetSaveFilename()
        {
            return await _filesRepository.Create();
        }

        public FileStream GetSaveFileStream(string filename)
        {
            return new FileStream(Path.Combine(_webRoot, filename), FileMode.Create);
        }

        public async Task<string> SaveFile(Stream file)
        {
            var filename = await GetSaveFilename();

            using (var fs = GetSaveFileStream(filename))
            {
                await file.CopyToAsync(fs);
            }

            return $"{UploadsDirectory}/{filename}";
        }

        public async Task<string> SaveFile(byte[] file)
        {
            using (var stream = new MemoryStream(file))
            {
                return await SaveFile(stream);   
            }
        }
    }
}








