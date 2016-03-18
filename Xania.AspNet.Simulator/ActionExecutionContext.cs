using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xania.AspNet.Core;

namespace Xania.AspNet.Simulator
{
    public class ActionExecutionContext
    {
        public virtual ControllerContext ControllerContext { get; set; }
        public virtual ActionDescriptor ActionDescriptor { get; set; }

        public ActionExecutionContext(IMvcApplication mvcApplication)
        {
            MvcApplication = mvcApplication;
        }

        public virtual ModelBinderDictionary ModelBinders
        {
            get { return MvcApplication.Binders; }
        }

        public virtual ControllerBase Controller
        {
            get { return ControllerContext.Controller; }
        }
        public virtual dynamic ViewBag
        {
            get { return ControllerContext.Controller.ViewBag; }
        }
        public virtual ViewDataDictionary ViewData
        {
            get { return ControllerContext.Controller.ViewData; }
        }
        public virtual TempDataDictionary TempData
        {
            get { return ControllerContext.Controller.TempData; }
        }

        public virtual ModelStateDictionary ModelState
        {
            get { return ControllerContext.Controller.ViewData.ModelState; }
        }

        public virtual HttpResponseBase Response
        {
            get { return ControllerContext.HttpContext.Response; }
        }

        public virtual HttpRequestBase Request
        {
            get { return ControllerContext.HttpContext.Request; }
        }

        public virtual RouteData RouteData
        {
            get { return ControllerContext.RequestContext.RouteData; }
        }

        public ViewEngineCollection ViewEngines
        {
            get { return MvcApplication.ViewEngines; }
        }

        public IMvcApplication MvcApplication { get; private set; }

        public virtual AuthorizationContext GetAuthorizationContext()
        {
            return new AuthorizationContext(ControllerContext, ActionDescriptor);
        }

        public virtual ActionExecutingContext GetActionExecutingContext()
        {
            return new ActionExecutingContext(ControllerContext, ActionDescriptor, GetParameterValues());
        }

        public virtual ActionExecutedContext GetActionExecutedContext(bool canceled = false, Exception exception = null)
        {
            return new ActionExecutedContext(ControllerContext, ActionDescriptor, canceled, exception);
        }

        public virtual ResultExecutingContext GetResultExecutingContext(ActionResult actionResult)
        {
            return new ResultExecutingContext(ControllerContext, actionResult);
        }

        public virtual ResultExecutedContext GetResultExecutedContext(ActionResult actionResult, bool canceled = false, Exception exception = null)
        {
            return new ResultExecutedContext(ControllerContext, actionResult, canceled, exception);
        }

        public virtual ExceptionContext GetExceptionContext(Exception exception)
        {
            return new ExceptionContext(ControllerContext, exception);
        }

        /// <summary>
        /// Gets the values of the action-method parameters.
        /// </summary>
        public virtual IDictionary<string, object> GetParameterValues()
        {
            var dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var parameterDescriptor in ActionDescriptor.GetParameters())
                dictionary[parameterDescriptor.ParameterName] = GetParameterValue(parameterDescriptor);
            return dictionary;
        }

        /// <summary>
        /// Gets the value of the specified action-method parameter.
        /// </summary>
        public virtual object GetParameterValue(ParameterDescriptor parameterDescriptor)
        {
            Type parameterType = parameterDescriptor.ParameterType;
            IModelBinder modelBinder = GetModelBinder(parameterDescriptor);
            IValueProvider valueProvider = ControllerContext.Controller.ValueProvider;
            string str = parameterDescriptor.BindingInfo.Prefix ?? parameterDescriptor.ParameterName;
            Predicate<string> propertyFilter = GetPropertyFilter(parameterDescriptor);
            ModelBindingContext bindingContext = new ModelBindingContext()
            {
                FallbackToEmptyPrefix = parameterDescriptor.BindingInfo.Prefix == null,
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType((Func<object>)null, parameterType),
                ModelName = str,
                ModelState = ControllerContext.Controller.ViewData.ModelState,
                PropertyFilter = propertyFilter,
                ValueProvider = valueProvider
            };
            return modelBinder.BindModel(ControllerContext, bindingContext) ?? parameterDescriptor.DefaultValue;
        }

        public virtual IModelBinder GetModelBinder(ParameterDescriptor parameterDescriptor)
        {
            return parameterDescriptor.BindingInfo.Binder ?? ModelBinders.GetBinder(parameterDescriptor.ParameterType);
        }

        private static Predicate<string> GetPropertyFilter(ParameterDescriptor parameterDescriptor)
        {
            var bindingInfo = parameterDescriptor.BindingInfo;
            return propertyName => IsPropertyAllowed(propertyName, bindingInfo.Include.ToArray(), bindingInfo.Exclude.ToArray());
        }

        private static bool IsPropertyAllowed(string propertyName, string[] includeProperties, string[] excludeProperties)
        {
            bool flag1 = includeProperties == null || includeProperties.Length == 0 || includeProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase);
            bool flag2 = excludeProperties != null && excludeProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase);
            if (flag1)
                return !flag2;
            return false;
        }
    }
}