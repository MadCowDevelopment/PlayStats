using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayStats.Data
{
    public class PlayAccessor : LiteDBAccessor<PlayEntity>
    {
    }

    public class GameAccessor : LiteDBAccessor<GameEntity>
    {
    }

    public class LinkedGameAccessor : LiteDBAccessor<LinkedGameEntity>
    {
    }

    public interface IDataAccessor<T> where T : Entity
    {
        void Create(T entity);
        void Update(T entity);
        void Delete(Guid id);
        IEnumerable<T> GetAll();
    }
    
    public abstract class LiteDBAccessor<T> : IDataAccessor<T> where T : Entity
    {
        private const string DatabaseFile = @"C:\Users\MGailer\OneDrive\Data\PlayStats\lite.db";
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
