using System;
using System.Collections.Concurrent;
using Loki.Interfaces.Dependency;

namespace Loki.Server.Dependency
{
    public class DependencyUtility: IDependencyUtility
    {
        /// <summary>
        /// The cache
        /// </summary>
        private readonly ConcurrentDictionary<Type, object> _cache = new ConcurrentDictionary<Type, object>();

        /// <summary>
        /// Registers the specified object.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="obj">The object.</param>
        public void Register<TInterface>(TInterface obj)
        {
            if (obj == null)
                return;

            _cache[typeof(TInterface)] = obj;
        }

        /// <summary>
        /// Resolves this instance.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <returns></returns>
        public TInterface Resolve<TInterface>()
        {
            Type t = typeof(TInterface);

            if (!_cache.ContainsKey(t))
                return default(TInterface);

            object value;

            int i = 0;
            while (!_cache.TryGetValue(t, out value))
            {
                ++i;

                if (i > 3)
                    return default(TInterface);
            }

            return (TInterface)value;
        }
    }
}
