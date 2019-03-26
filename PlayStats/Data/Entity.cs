using System;
using System.Reflection;

namespace PlayStats.Data
{
    public class Entity
    {
        public Guid Id { get; set; }

        internal void SetProperties(Entity entity)
        {
            if (entity == null) throw new InvalidOperationException("The entity cannot be null.");
            if (GetType() != entity.GetType()) throw new InvalidOperationException("Cannot set properties on different types of entities.");

            var properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach(var prop in properties)
            {
                if (prop.Name == "Id") continue;
                var sourceValue = prop.GetValue(entity);
                prop.SetValue(this, sourceValue);
            }
        }
    }
}