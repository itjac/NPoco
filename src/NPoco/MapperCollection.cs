using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace NPoco
{
    public class MapperCollection : List<IMapper>
    {
        internal readonly Dictionary<Type, ObjectFactoryDelegate> Factories = new Dictionary<Type, ObjectFactoryDelegate>();

        public delegate object ObjectFactoryDelegate(IDataReader dataReader);

        public void RegisterFactory<T>(Func<IDataReader, T> factory)
        {
            Factories[typeof(T)] = x => factory(x);
        }

        public ObjectFactoryDelegate GetFactory(Type type)
        {
            return Factories.ContainsKey(type) ? Factories[type] : null;
        }

        public bool HasFactory(Type type)
        {
            return Factories.ContainsKey(type);
        }

        public void ClearFactories(Type type = null)
        {
            if (type != null && HasFactory(type))
                Factories.Remove(type);
            else
                Factories.Clear();
        }

        public Func<object, object> Find(Func<IMapper, Func<object, object>> predicate)
        {
            return this.Select(predicate).FirstOrDefault(x => x != null);
        }

        public object FindAndExecute(Func<IMapper, Func<object, object>> predicate, object value)
        {
            var converter = Find(predicate);
            return converter != null ? converter(value) : value;
        }
    }
}