using DynamicData;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace PlayStats.Utils
{
    public static class DynamicDataExtensions
    {
        public static IObservable<TObject> WhenAnyPropertyWithAttributeChanged<TObject, TKey, TAttribute>(this IObservable<IChangeSet<TObject, TKey>> source) 
            where TObject : INotifyPropertyChanged
            where TAttribute : Attribute
        {
            return source.WhenAnyPropertyWithAttributeChanged(typeof(TAttribute));
        }

        public static IObservable<TObject> WhenAnyPropertyWithAttributeChanged<TObject, TKey>(this IObservable<IChangeSet<TObject, TKey>> source, Type attributeType)
            where TObject : INotifyPropertyChanged
        {
            var props = typeof(TObject).GetProperties(BindingFlags.Public | BindingFlags.Instance)
               .Where(p => p.GetCustomAttribute(attributeType) != null).Select(p => p.Name).ToArray();
            return source.WhenAnyPropertyChanged(props);
        }
    }
}
