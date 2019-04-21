using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Achiever.Core.DbInterfaces;
using Achiever.Core.Models.User;
using MongoDB.Driver;

namespace Achiever.Data.Repositories
{   
    public class UserRepository : IUserRepository
    {
        private readonly MongoContext _context;

        public UserRepository(MongoContext context)
        {
            _context = context;
            CreateIndexes();
        }

        public Task<User> GetById(string id)
        {
            return _context.For<User>()
                .Find(x => x.Id == id)
                .SingleOrDefaultAsync();
        }

        public Task<string> GetFirebaseToken(string userId)
        {
            return _context.For<User>()
                .Find(x => x.Id == userId)
                .Project(x => x.FirebaseToken)
                .SingleOrDefaultAsync();
        }

        public Task SetFirebaseToken(string userId, string firebaseToken)
        {
            return _context.For<User>()
                .UpdateOneAsync(x => x.Id == userId,
                    Builders<User>.Update.Set(x => x.FirebaseToken, firebaseToken));
        }

        public Task<User> GetByNickname(string nickname)
        {
            return _context.For<User>()
                .Find(x => x.Nickname == nickname)
                .SingleOrDefaultAsync();
        }

        public Task<User> GetByCredentials(string name, string password)
        {
            return _context.For<User>()
                .Find(x => x.Nickname == name && x.Password == password)
                .SingleOrDefaultAsync();
        }

        public async Task Create(string nickname, string password)
        {
            try
            {
                await _context.For<User>()
                    .InsertOneAsync(new User
                    {
                        Nickname = nickname,
                        Password = password,
                        ProfileImagePath = "default.png"
                    });
            }
            catch (MongoWriteException)
            {
                throw new Exception("Никнейм занят");
            }
        }

        public Task UpdateProfileImage(string userId, string profileImagePath)
        {
            var filter = Builders<User>.Filter.Where(x => x.Id == userId);

            var update = Builders<User>.Update.Set(x => x.ProfileImagePath, profileImagePath);
            
            return _context.For<User>().UpdateOneAsync(filter, update);
        }

        public Task<List<User>> GetByIds(List<string> ids)
        {
            return _context.For<User>()
                .Find(x => ids.Contains(x.Id))
                .Project(y => new User
                {
                    Id = y.Id,
                    Nickname = y.Nickname,
                    ProfileImagePath = y.ProfileImagePath
                })
                .ToListAsync();
        }

        public Task Follow(string userId, string targetId)
        {
            var userFilter = Builders<User>.Filter.Where(x => x.Id == userId);
            var userUpdate = Builders<User>.Update.AddToSet(x => x.FollowingIds, targetId);
                //.Inc(x => x.Stats.Following, 1);
            var userTask = _context.For<User>().UpdateOneAsync(userFilter, userUpdate);

            var targetFilter = Builders<User>.Filter.Where(x => x.Id == targetId);
            var targetUpdate = Builders<User>.Update.AddToSet(x => x.FollowersIds, userId);
                //.Inc(x => x.Stats.Followers, 1);
            var targetTask = _context.For<User>().UpdateOneAsync(targetFilter, targetUpdate);

            return Task.WhenAll(userTask, targetTask);
        }

        public Task Unfollow(string userId, string targetId)
        {
            var userFilter = Builders<User>.Filter.Where(x => x.Id == userId);
            var userUpdate = Builders<User>.Update
                .Pull(x => x.FollowingIds, targetId);
                //.Inc(x => x.Stats.Following, -1);
            var userTask = _context.For<User>().UpdateOneAsync(userFilter, userUpdate);

            var targetFilter = Builders<User>.Filter.Where(x => x.Id == targetId);
            var targetUpdate = Builders<User>.Update
                .Pull(x => x.FollowersIds, userId);
                //.Inc(x => x.Stats.Followers, -1);
            var targetTask = _context.For<User>().UpdateOneAsync(targetFilter, targetUpdate);

            return Task.WhenAll(userTask, targetTask);
        }

        public Task<User> Update(string userId, User model)
        {
            var filter = Builders<User>.Filter.Where(x => x.Id == userId);

            var update = Builders<User>.Update.Set(x => x.Id, userId);

            if (!string.IsNullOrEmpty(model.Nickname))
                update = update.Set(x => x.Nickname, model.Nickname);

            if (!string.IsNullOrEmpty(model.About))
                update = update.Set(x => x.About, model.About);

            if (!string.IsNullOrEmpty(model.ProfileImagePath))
                update = update.Set(x => x.ProfileImagePath, model.ProfileImagePath);

            if (!string.IsNullOrEmpty(model.DisplayName))
                update = update.Set(x => x.DisplayName, model.DisplayName);

            return _context.For<User>()
                .FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<User>
                {
                    ReturnDocument = ReturnDocument.After,
                    Projection = Builders<User>.Projection.Exclude(x => x.Password)
                });
        }

        public Task<List<User>> GetAll()
        {
            return _context.For<User>()
                .Find(_ => true)
                .ToListAsync();
        }

        private void CreateIndexes()
        {
            _context.For<User>().Indexes.CreateMany(
                new []
                {
                    new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(x => x.Nickname), new CreateIndexOptions
                    {
                        Unique = true
                    })
                });
        }
    }
}