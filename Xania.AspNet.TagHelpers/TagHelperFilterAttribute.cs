using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;

namespace Xania.AspNet.TagHelpers
{
    public class TagHelperFilterAttribute : ActionFilterAttribute
    {
        private readonly IDependencyResolver _defaultDependencyResolver;

        public TagHelperFilterAttribute()
            : this(DependencyResolver.Current)
        {
        }

        private TagHelperFilterAttribute(IDependencyResolver defaultDependencyResolver)
        {
            _defaultDependencyResolver = defaultDependencyResolver;
        }


        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            filterContext.Result = GetFilterResult(filterContext);
        }

        protected virtual ActionResult GetFilterResult(ActionExecutedContext filterContext)
        {
            return new TagHelperFilterResult(_defaultDependencyResolver, filterContext.Result, GetCustomTagHelpers());
        }

        protected virtual IEnumerable<KeyValuePair<string, Type>> GetCustomTagHelpers()
        {
            return Enumerable.Empty<KeyValuePair<string, Type>>();
        }
    }

    public class TagHelperServiceContainer : IDependencyResolver
    {
        private readonly IDependencyResolver _mainResolver;
        private readonly ControllerContext _controllerContext;
        private readonly UnityContainer _unityContainer;

        public TagHelperServiceContainer(IDependencyResolver mainResolver, ControllerContext controllerContext)
        {
            _mainResolver = mainResolver;
            _controllerContext = controllerContext;
            _unityContainer = new UnityContainer();
            _unityContainer.RegisterInstance(typeof(RequestContext), controllerContext.RequestContext);
            _unityContainer.RegisterInstance(typeof(UrlHelper), new UrlHelper(controllerContext.RequestContext));
            _unityContainer.RegisterInstance(typeof(ControllerContext), _controllerContext);
        }

        public object GetService(Type serviceType)
        {
            return _unityContainer.Resolve(serviceType, new MainResolverOverride(_mainResolver));
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _unityContainer.ResolveAll(serviceType, new MainResolverOverride(_mainResolver));
        }
    }

    public class OptionalOverride : ResolverOverride
    {
        public override IDependencyResolverPolicy GetResolver(IBuilderContext context, Type dependencyType)
        {
            return new OptionalDependencyResolverPolicy(dependencyType);
        }
    }

    public class MainResolverOverride : ResolverOverride, IDependencyResolverPolicy
    {
        private readonly IDependencyResolver _mainResolver;
        private Type _dependencyType;

        public MainResolverOverride(IDependencyResolver mainResolver)
        {
            _mainResolver = mainResolver;
        }

        public override IDependencyResolverPolicy GetResolver(IBuilderContext context, Type dependencyType)
        {
            _dependencyType = dependencyType;
            return this;
        }

        public object Resolve(IBuilderContext context)
        {
            return null;
        }
    }
}