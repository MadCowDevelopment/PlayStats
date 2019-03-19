using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayStats.Data
{
    public class PlayAccessor : LiteDBAccessor<Play>
    {
    }

    public class GameAccessor : LiteDBAccessor<Game>
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
        private readonly string CollectionName = $"{typeof(T).Name.ToLower().Replace("Entity", string.Empty)}s";

        public void Create(T entity)
        {
            Execute(col => col.Insert(entity));
        }

        public void Update(T entity)
        {
            Execute(col =>
            {
                var existingEntity = col.FindById(new BsonValue(entity.Id));
                if (existingEntity == null) return;
                existingEntity.SetProperties(entity);
                col.Update(entity);
            });
        }

        public void Delete(Guid id)
        {
            Execute(col =>
            {
                col.Delete(id);
            });
        }

        public IEnumerable<T> GetAll()
        {
            return Execute(col => col.FindAll().ToList());
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
