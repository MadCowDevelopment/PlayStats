using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace PlayStats.Data
{
    public class MongoDbPlayAccessor : MongoDBAccessor<PlayEntity> { }
    public class MongoDbGameAccessor : MongoDBAccessor<GameEntity> { }
    public class MongoDbLinkedGameAccessor : MongoDBAccessor<LinkedGameEntity> { }

    public abstract class MongoDBAccessor<T> : IDataAccessor<T> where T : Entity
    {
        private readonly string CollectionName = $"{typeof(T).Name.ToLower().Replace("entity", string.Empty)}s";
        private readonly IMongoDatabase db;

        public MongoDBAccessor()
        {
            var client = new MongoClient("mongodb+srv://USER:PASSWORD@madcowdev-7axwk.azure.mongodb.net/test?retryWrites=true&w=majority");
            db = client.GetDatabase("PlayStats");
        }

        public void Create(T entity)
        {
            Execute(col => col.InsertOne(entity));
        }

        public void Update(T entity)
        {
            Execute(col =>
            {
                var existingEntity = col.Find(p => p.Id == entity.Id).SingleOrDefault();
                if (existingEntity == null) throw new InvalidOperationException($"Could not find entity with ID {entity.Id} when trying to update in collection '{CollectionName}'.");
                existingEntity.SetProperties(entity);
                col.ReplaceOne(p => p.Id == entity.Id, existingEntity);
            });
        }

        public void Delete(Guid id)
        {
            Execute(col => col.DeleteOne(p => p.Id == id));
        }

        public IEnumerable<T> GetAll()
        {
            return Execute(col => col.Find(FilterDefinition<T>.Empty).ToList());
        }
        
        private void Execute(Action<IMongoCollection<T>> action)
        {
            var col = db.GetCollection<T>(CollectionName);
            action(col);
        }

        private IEnumerable<T> Execute(Func<IMongoCollection<T>, IEnumerable<T>> action)
        {
            var col = db.GetCollection<T>(CollectionName);
            return action(col);
        }
    }
}
