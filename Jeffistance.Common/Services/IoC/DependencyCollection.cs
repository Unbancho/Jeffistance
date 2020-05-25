using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Jeffistance.Common.Services.IoC
{
    internal class DependencyCollection
    {
        /// <summary>
        /// Dictionary that contains types and their implementations.
        /// Pulled from to make services.
        /// </summary>
        private readonly Dictionary<Type, Type> _resolveTypes = new Dictionary<Type, Type>();

        /// <summary>
        /// Dictionary that maps types passed to <see cref="Resolve{T}"/> to their implementation.
        /// </summary>
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        /// <summary>
        /// Register a type to its implementation.
        /// </summary>
        /// <typeparam name="TInterface">The type that will be resolvable.</typeparam>
        /// <typeparam name="TImplementation">
        /// The type that will be constructed as implementation.
        /// </typeparam>
        public void Register<TInterface, TImplementation>()
            where TImplementation : class, TInterface, new()
        {
            var interfaceType = typeof(TInterface);

            if (_resolveTypes.ContainsKey(interfaceType))
            {
                throw new InvalidOperationException(
                    $"Attempted to register already registered interface {interfaceType}"
                );
            }

            if (_services.ContainsKey(interfaceType))
            {
                throw new InvalidOperationException(
                    $"Attempted to overwrite already instantiated interface {interfaceType}"
                );
            }

            _resolveTypes[interfaceType] = typeof(TImplementation);
        }

        /// <summary>
        /// Resolve given dependency.
        /// </summary>
        /// <typeparam name="T">The interface that will be resolved.</typeparam>
        public T Resolve<T>()
        {
            return (T) ResolveType(typeof(T));
        }

        public object ResolveType(Type type)
        {
            if (_services.TryGetValue(type, out var value))
            {
                return value;
            }

            if (_resolveTypes.ContainsKey(type))
            {
                throw new InvalidOperationException(
                    $"Attempted to resolve type {type} before the object graph has been populated."
                );
            }

            throw new UnregisteredTypeException(type);
        }

        /// <summary>
        /// Build all the necessary dependencies.
        /// </summary>
        public void BuildGraph()
        {
            foreach (var (key, value) in _resolveTypes.Where(p => !_services.ContainsKey(p.Key)))
            {
                try
                {
                    var instance = Activator.CreateInstance(value);
                    _services[key] = instance;
                }
                catch (TargetInvocationException e)
                {
                    throw new InvalidOperationException(e.Message);
                }
            }
        }

        /// <summary>
        /// Clear all registered interfaces and implementations.
        /// </summary>
        public void Clear()
        {
            foreach (var service in _services.Values.OfType<IDisposable>().Distinct())
            {
                service.Dispose();
            }

            _services.Clear();
            _resolveTypes.Clear();
        }
    }
}
