using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public class ControllerContainer : Core.IControllerFactory
    {
        private readonly Dictionary<string, IValueProvider<ControllerBase>> _controllerMap;

        public ControllerContainer()
        {
            _controllerMap = new Dictionary<String, IValueProvider<ControllerBase>>(StringComparer.InvariantCultureIgnoreCase);
        }

        public virtual ControllerContainer RegisterController(string name, ControllerBase controller)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            _controllerMap.Add(name, new LiteralValueProvider<ControllerBase>(controller));

            return this;
        }

        public virtual ControllerContainer RegisterController(string name, Func<ControllerBase> controllerFactory)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            _controllerMap.Add(name, new FactoryValueProvider<ControllerBase>(controllerFactory));

            return this;
        }

        public virtual ControllerBase CreateController(string controllerName)
        {
            IValueProvider<ControllerBase> controllerProvider;
            if (_controllerMap.TryGetValue(controllerName, out controllerProvider))
                return controllerProvider.Value;

            throw new HttpException(404, "Controller '" + controllerName + "' not found");
        }

        interface IValueProvider<out TValue>
        {
            TValue Value { get; }
        }

        class FactoryValueProvider<TValue> : IValueProvider<TValue>
        {
            private readonly Func<TValue> _factory;

            public FactoryValueProvider(Func<TValue> factory)
            {
                _factory = factory;
            }

            public TValue Value
            {
                get { return _factory(); }
            }
        }

        class LiteralValueProvider<TValue> : IValueProvider<TValue>
        {
            public LiteralValueProvider(TValue value)
            {
                Value = value;
            }

            public TValue Value { get; private set; }
        }
    }
}
