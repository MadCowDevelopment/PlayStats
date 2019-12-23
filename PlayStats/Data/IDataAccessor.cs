using System;
using System.Collections.Generic;

namespace PlayStats.Data
{
    public interface IDataAccessor<T> where T : Entity
    {
        void Create(T entity);
        void Update(T entity);
        void Delete(Guid id);
        IEnumerable<T> GetAll();
    }
}