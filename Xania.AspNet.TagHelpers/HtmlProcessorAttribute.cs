using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Xania.AspNet.TagHelpers
{
    public class HtmlProcessorAttribute : ActionFilterAttribute
    {
        private readonly ITagHelperProvider _tagHelperProvider;

        public HtmlProcessorAttribute(ITagHelperProvider tagHelperProvider)
        {
            _tagHelperProvider = tagHelperProvider;
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var response = filterContext.HttpContext.Response;
            if (response.Filter != null)
            {
                var writer = new StreamWriter(response.Filter, response.ContentEncoding);
                response.Filter = new HtmlProcessor(writer, GetTagHelperProvider(filterContext));
            }
            base.OnResultExecuting(filterContext);
        }

        private ITagHelperProvider GetTagHelperProvider(ResultExecutingContext filterContext)
        {
            if (filterContext.Result is ViewResult)
            {
                var viewResult = filterContext.Result as ViewResult;
                var razorDecorator = new RazorTagHelperProvider(_tagHelperProvider);
                razorDecorator.Register(viewResult.ViewData);
            }
            return _tagHelperProvider;
        }

        class RazorTagHelperProvider: ITagHelperProvider
        {
            private readonly ITagHelperProvider _inner;
            private readonly IDictionary<Type, object> _services;

            public RazorTagHelperProvider(ITagHelperProvider inner)
            {
                _inner = inner;
                _services = new Dictionary<Type, object>();
            }

            public void Register<TObject>(TObject instance)
            {
                _services.Add(typeof (TObject), instance);
            }

            public ITagHelper GetTagHelper(string tagName, IDictionary<string, string> attributes)
            {
                var tagHelper = _inner.GetTagHelper(tagName, attributes);

                BindProperties(tagHelper);

                return tagHelper;
            }

            private void BindProperties(object instance)
            {
                var objectType = instance.GetType();

                foreach (var kvp in _services)
                {
                    var serviceType = kvp.Key;
                    foreach (var propertyInfo in objectType.GetProperties()
                        .Where(prop => prop.PropertyType.IsAssignableFrom(serviceType)))
                    {
                        propertyInfo.SetValue(instance, kvp.Value);
                    }
                }
            }
        }
    }
}