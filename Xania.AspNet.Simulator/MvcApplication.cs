using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Xania.AspNet.Simulator
{
    public class MvcApplication
    {
        private readonly Dictionary<string, ControllerBase> _controllerMap;
        private readonly RouteCollection _routes;

        public MvcApplication()
        {
            _controllerMap = new Dictionary<String, ControllerBase>();
            _routes = new RouteCollection();
        }

        public void RegisterController(string name, ControllerBase controller)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            _controllerMap.Add(name.ToLower(CultureInfo.InvariantCulture), controller);
        }

        protected internal virtual ControllerBase CreateController(string controllerName)
        {
            ControllerBase controller;
            if (_controllerMap.TryGetValue(controllerName.ToLower(CultureInfo.InvariantCulture), out controller))
                return controller;

            throw new KeyNotFoundException(controllerName);
        }

        public ControllerAction Action(string url)
        {
            var context = AspNetUtility.GetContext(url, null);
            var routeData = _routes.GetRouteData(context);

            if (routeData == null)
                return null;

            var controllerName = routeData.GetRequiredString("controller");
            var controller = CreateController(controllerName);
            var controllerDescriptor = new ReflectedControllerDescriptor(controller.GetType());
            var actionDescriptor = controllerDescriptor.FindAction(new ControllerContext(context, routeData, controller), routeData.GetRequiredString("action"));

            return new ControllerAction(controller, actionDescriptor);
        }

        public MvcApplication RegisterRoutes(Action<RouteCollection> configAction)
        {
            configAction(_routes);
            return this;
        }

        public MvcApplication RegisterDefaultRoutes()
        {
            _routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            return this;
        }
    }
}
