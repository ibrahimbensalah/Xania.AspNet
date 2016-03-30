using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public class ControllerContainer : Core.IControllerFactory
    {
        private readonly IDictionary<ControllerKey, ControllerFactory> _controllerMap;

        public ControllerContainer()
        {
            _controllerMap = new Dictionary<ControllerKey, ControllerFactory>();
        }

        public ControllerContainer RegisterController(string controllerName, Func<ControllerBase> controllerFactory)
        {
            return RegisterController(controllerName, ctx => controllerFactory());
        }

        public ControllerContainer RegisterController(string controllerName, Func<HttpContextBase, ControllerBase> controllerFactory)
        {
            return RegisterController(controllerName, null, controllerFactory);
        }

        public virtual ControllerContainer RegisterController(string controllerName, string areaName,
            Func<ControllerBase> controllerFactory)
        {
            return RegisterController(controllerName, areaName, ctx => controllerFactory());
        }

        public virtual ControllerContainer RegisterController(string controllerName, string areaName,
            Func<HttpContextBase, ControllerBase> controllerFactory)
        {
            if (string.IsNullOrEmpty(controllerName))
                throw new ArgumentNullException("controllerName");

            var key = new ControllerKey(controllerName, areaName);

            _controllerMap.Add(key, new ControllerFactory(controllerFactory));

            return this;
        }

        public virtual ControllerBase CreateController(HttpContextBase context, String controllerName)
        {
            // var areaName = context.Request.RequestContext.RouteData.DataTokens["area"] as string;

            var key = new ControllerKey(controllerName, null);
            ControllerFactory factory;
            if (_controllerMap.TryGetValue(key, out factory))
                return factory.Create(context);

            throw new HttpException(404, "Controller '" + controllerName + "' not found");
        }

        public static implicit operator ControllerContainer(ControllerBase controller)
        {
            var controllerType = controller.GetType();
            var name = controllerType.Name.Substring(0, controllerType.Name.Length - "Controller".Length);

            return new ControllerContainer()
                .RegisterController(name, ctx => controller);
        }

        private struct ControllerKey
        {
            public ControllerKey(string name, string area)
                : this()
            {
                Name = name;
                Area = area;

                if (name == null)
                    throw new ArgumentNullException("name");

            }

            public string Name { get; private set; }
            public string Area { get; private set; }

            public override bool Equals(object obj)
            {
                if (!(obj is ControllerKey)) 
                    return false;

                var other = (ControllerKey) obj;
                return string.Equals(other.Name, Name, StringComparison.InvariantCultureIgnoreCase) && 
                    string.Equals(other.Area, Area, StringComparison.InvariantCultureIgnoreCase);
            }

            public override int GetHashCode()
            {
                var hashCode = Name.ToLowerInvariant().GetHashCode();
                if (!string.IsNullOrEmpty(Area))
                    hashCode += Area.ToLowerInvariant().GetHashCode();
                return hashCode;
            }
        }

        private class ControllerFactory
        {
            private readonly Func<HttpContextBase, ControllerBase> _factory;

            public ControllerFactory(Func<HttpContextBase, ControllerBase> factory)
            {
                _factory = factory;
            }

            public ControllerBase Create(HttpContextBase context)
            {
                return _factory(context);
            }
        }
    }
}
