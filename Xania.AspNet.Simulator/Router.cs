using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Routing;

namespace Xania.AspNet.Simulator
{
    public class Router
    {
        private readonly Dictionary<string, ControllerBase> _controllerMap;
        public RouteCollection Routes { get; private set; }

        public Router()
        {
            _controllerMap = new Dictionary<String, ControllerBase>();
            Routes = new RouteCollection(new ActionRouterPathProvider());
        }

        public virtual Router RegisterController(string name, ControllerBase controller)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            _controllerMap.Add(name.ToLower(CultureInfo.InvariantCulture), controller);

            return this;
        }

        protected internal virtual ControllerBase CreateController(string controllerName)
        {
            ControllerBase controller;
            if (_controllerMap.TryGetValue(controllerName.ToLower(CultureInfo.InvariantCulture), out controller))
                return controller;

            throw new KeyNotFoundException(controllerName);
        }

        public virtual Router RegisterDefaultRoutes()
        {
            Routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            return this;
        }
    }
}
