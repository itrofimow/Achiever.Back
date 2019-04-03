using System;
using System.Linq;
using System.Threading.Tasks;
using Achiever.Core.DbInterfaces;
using Achiever.Core.Models.Test;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Achiever.Data.Repositories
{
    public class TestRepository : ITestRepository
    {
        private readonly MongoContext _context;

        public TestRepository(MongoContext context)
        {
            _context = context;
        }

        public Task InsertB(ModelB model)
        {
            return _context.For<ModelB>()
                .InsertOneAsync(model);
        }

        public Task InsertA(ModelA model)
        {
            return _context.For<ModelA>()
                .InsertOneAsync(model);
        }

        public Task<ModelA> Get(string id)
        {
            var filter = Builders<ModelA>.Filter.Where(x => x.Id == id);
            
            return _context.For<ModelA>().Aggregate()
                .Match(filter)
                .Lookup(
                    foreignCollection: _context.For<ModelB>(),
                    localField: l => l.ModelBId,
                    foreignField: f => f.Id,
                    @as: (ModelAWithBs eo) => eo.Bs)
                .Project(p => new ModelA
                {
                    Id = p.Id,
                    ModelBId = p.ModelBId,
                    B = p.Bs.First()
                })
                .SingleOrDefaultAsync();
            
            //Lookup()
            //return _context.For<ModelA>().
        }
    }
}