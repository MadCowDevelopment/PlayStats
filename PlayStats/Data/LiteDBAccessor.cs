using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PlayStats.Data
{
    public class LiteDbPlayAccessor : LiteDBAccessor<PlayEntity> { }

    public class LiteDbGameAccessor : LiteDBAccessor<GameEntity> { }

    public class LiteDbLinkedGameAccessor : LiteDBAccessor<LinkedGameEntity> { }

    public abstract class LiteDBAccessor<T> : IDataAccessor<T> where T : Entity
    {
        private readonly string DatabaseFile =
            Path.Combine(Environment.GetEnvironmentVariable("OneDrive"), @"Data\PlayStats\lite.db");

        private readonly string CollectionName = $"{typeof(T).Name.ToLower().Replace("entity", string.Empty)}s";

        public void Create(T entity)
        {
            Execute(col => col.Insert(entity));
        }

        public void Update(T entity)
        {
            Execute(col =>
            {
                var existingEntity = col.FindById(new BsonValue(entity.Id));
                if (existingEntity == null) throw new InvalidOperationException($"Could not find entity with ID {entity.Id} when trying to update in collection '{CollectionName}'.");
                existingEntity.SetProperties(entity);
                col.Update(entity);
            });
        }

        public void Delete(Guid id)
        {
            Execute(col => col.Delete(id));
        }

        public IEnumerable<T> GetAll()
        {
            return Execute(col => col.FindAll().AsEnumerable());
        }

        private LiteDatabase CreateLiteDatabase()
        {
            return new LiteDatabase(DatabaseFile);
        }

        private void Execute(Action<LiteCollection<T>> action)
        {
            using (var db = CreateLiteDatabase())
            {
                var col = db.GetCollection<T>(CollectionName);
                action(col);
            }
        }

        private IEnumerable<T> Execute(Func<LiteCollection<T>, IEnumerable<T>> action)
        {
            using (var db = CreateLiteDatabase())
            {
                var col = db.GetCollection<T>(CollectionName);
                return action(col);
            }
        }
    }
}
