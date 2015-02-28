using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Web.Mvc;
using System.Web.Routing;

namespace Xania.AspNet.TagHelpers
{
    public class TagHelperFilterResult : ActionResult
    {
        private readonly IDependencyResolver _defaultDependencyResolver;
        private readonly ActionResult _actionResult;
        private readonly IEnumerable<KeyValuePair<string, Type>> _customTagHandlers;

        public TagHelperFilterResult(IDependencyResolver defaultDependencyResolver, ActionResult actionResult, IEnumerable<KeyValuePair<string, Type>> customTagHandlers)
        {
            _defaultDependencyResolver = defaultDependencyResolver;
            _actionResult = actionResult;
            _customTagHandlers = customTagHandlers;
        }

        public override void ExecuteResult(ControllerContext controllerContext)
        {
            var tagHelperProvider = GetTagHelperProvider(new ActionDependencyResolver(_defaultDependencyResolver, controllerContext));

            var response = controllerContext.HttpContext.Response;
            if (response.Filter != null)
            {
                response.Filter = new HtmlProcessor(new StreamWriter(response.Filter, response.ContentEncoding),
                    tagHelperProvider);
            }
            else
            {
                response.Output = new TagHelperWriter(response.Output, tagHelperProvider);
            }
            _actionResult.ExecuteResult(controllerContext);
        }

        protected virtual ITagHelperProvider GetTagHelperProvider(IDependencyResolver resolver)
        {
            var helperContainer = new TagHelperContainer(resolver.GetService);
            foreach (var kvp in GetTagHandlers())
                helperContainer.Register(kvp.Key, kvp.Value);

            if (_customTagHandlers != null)
            {
                foreach (var kvp in _customTagHandlers)
                    helperContainer.Register(kvp.Key, kvp.Value);
            }

            return helperContainer;
        }

        protected virtual IEnumerable<KeyValuePair<string, Type>> GetTagHandlers()
        {
            yield return new KeyValuePair<string, Type>("a", typeof(AnchorTagHelper));
        }
    }

    public class ActionDependencyResolver : IDependencyResolver
    {
        private readonly IDependencyResolver _defaultDependencyResolver;
        private readonly ControllerContext _controllerContext;

        public ActionDependencyResolver(IDependencyResolver defaultDependencyResolver, ControllerContext controllerContext)
        {
            _defaultDependencyResolver = defaultDependencyResolver;
            _controllerContext = controllerContext;
        }

        public object GetService(Type serviceType)
        {
            if (typeof (ControllerContext) == serviceType)
                return _controllerContext;

            if (typeof (UrlHelper) == serviceType)
                return new UrlHelper(_controllerContext.RequestContext);

            if (typeof (RequestContext) == serviceType)
                return _controllerContext.RequestContext;

            return _defaultDependencyResolver.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}