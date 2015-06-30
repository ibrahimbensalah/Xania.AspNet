using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public class ControllerContainer : Core.IControllerFactory
    {
        private readonly Dictionary<string, IValueProvider<HttpContextBase, ControllerBase>> _controllerMap;

        public ControllerContainer()
        {
            _controllerMap = new Dictionary<String, IValueProvider<HttpContextBase, ControllerBase>>(StringComparer.InvariantCultureIgnoreCase);
        }

        public virtual ControllerContainer RegisterController(string name, ControllerBase controller)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            _controllerMap.Add(name, new LiteralValueProvider<ControllerBase>(controller));

            return this;
        }

        public virtual ControllerContainer RegisterController(string name, Func<HttpContextBase, ControllerBase> controllerFactory)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            _controllerMap.Add(name, new FactoryValueProvider<ControllerBase>(controllerFactory));

            return this;
        }

        public virtual ControllerBase CreateController(HttpContextBase context, String controllerName)
        {
            IValueProvider<HttpContextBase, ControllerBase> controllerProvider;
            if (_controllerMap.TryGetValue(controllerName, out controllerProvider))
                return controllerProvider.GetValue(context);

            throw new HttpException(404, "Controller '" + controllerName + "' not found");
        }

        interface IValueProvider<in TContext, out TValue>
        {
            TValue GetValue(TContext context);
        }

        class FactoryValueProvider<TValue> : IValueProvider<HttpContextBase, TValue>
        {
            private readonly Func<HttpContextBase, TValue> _factory;

            public FactoryValueProvider(Func<HttpContextBase, TValue> factory)
            {
                _factory = factory;
            }

            public TValue GetValue(HttpContextBase context)
            {
                return _factory(context);
            }
        }

        class LiteralValueProvider<TValue> : IValueProvider<HttpContextBase, TValue>
        {
            private readonly TValue _instance;

            public LiteralValueProvider(TValue value)
            {
                _instance = value;
            }

            public TValue GetValue(HttpContextBase context)
            {
                return _instance;
            }
        }

        public static implicit operator ControllerContainer(ControllerBase controller)
        {
            var controllerType = controller.GetType();
            var name = controllerType.Name.Substring(0, controllerType.Name.Length - "Controller".Length);

            return new ControllerContainer()
                .RegisterController(name, controller);
        }
    }
}
