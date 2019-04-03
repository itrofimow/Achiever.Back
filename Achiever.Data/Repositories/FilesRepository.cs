using System.Threading.Tasks;
using Achiever.Common;
using Achiever.Core.DbInterfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

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
    }

    [MongoEntity("Files")]
    internal class DbFile
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}