using System.Threading.Tasks;
using Achiever.Common;
using Achiever.Core.DbInterfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Achiever.Data.Repositories
{
    public class FilesRepository : IFilesRepository
    {
        private readonly MongoContext _context;

        public FilesRepository(MongoContext context)
        {
            _context = context;
        }

        public async Task<string> Create()
        {
            var file = new DbFile();
            await _context.For<DbFile>()
                .InsertOneAsync(file);

            return file.Id;
        }

        public async Task<string> StartImageCompression()
        {
            var filter = Builders<DbFile>.Filter.Where(
                x => x.Status == ImageProcessingStatus.Ready &&
                     x.PathToCompressed == null);

            var update = Builders<DbFile>.Update.Set(
                x => x.Status, ImageProcessingStatus.Processing);

            var file = await _context.For<DbFile>()
                .FindOneAndUpdateAsync(filter, update);

            return file?.Id;
        }

        public Task FinishImageCompression(string id, string compressedImagePath)
        {
            var filter = Builders<DbFile>.Filter.Where(
                x => x.Id == id);

            var update = Builders<DbFile>.Update
                .Set(x => x.Status,ImageProcessingStatus.Done)
                .Set(x => x.PathToCompressed, compressedImagePath);

            return _context.For<DbFile>()
                .UpdateOneAsync(filter, update);
        }
    }

    [MongoEntity("Files")]
    internal class DbFile
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string PathToCompressed { get; set; }

        public ImageProcessingStatus Status { get; set; }
    }

    internal enum ImageProcessingStatus
    {
        Ready = 0,
        
        Processing = 1,
        
        Done = 2
    }
}