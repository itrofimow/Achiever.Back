using System;

namespace Achiever.Common
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MongoEntity : Attribute
    {
        public string CollectionName { get; }

        public MongoEntity(string collectionName)
        {
            CollectionName = collectionName;
        }
    }
}