using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public interface IDependencyResolverFactory
    {
        IDependencyResolver Create(HttpContextBase httpContext);
    }

    public class DefaultDependencyResolverFactory : IDependencyResolverFactory
    {
        public IDependencyResolver Create(HttpContextBase httpContext)
        {
            return new DefaultDependencyResolver();
        }
    }

    public class DefaultDependencyResolver : IDependencyResolver
    {
        private readonly IDictionary<Type, object> _instances;


        public DefaultDependencyResolver()
        {
            _instances = new Dictionary<Type, object>();
        }

        public object GetService(Type serviceType)
        {
            object service;
            if (_instances.TryGetValue(serviceType, out service))
                return service;

            foreach (var i in _instances)
            {
                if (i.Key.IsAssignableFrom(serviceType))
                    return i.Value;
            }

            return Activator.CreateInstance(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            yield return GetService(serviceType);
        }

        public void Register<TService>(TService instance)
        {
            var serviceType = typeof (TService);
            _instances[serviceType] = instance;
        }
    }
}
