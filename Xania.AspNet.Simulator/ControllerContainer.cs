using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public class ControllerContainer : Core.IControllerFactory
    {
        private readonly Dictionary<string, ControllerBase> _controllerMap;

        public ControllerContainer()
        {
            _controllerMap = new Dictionary<String, ControllerBase>();
        }

        public virtual ControllerContainer RegisterController(string name, ControllerBase controller)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            _controllerMap.Add(name.ToLower(CultureInfo.InvariantCulture), controller);

            return this;
        }

        public virtual ControllerBase CreateController(string controllerName)
        {
            ControllerBase controller;
            if (_controllerMap.TryGetValue(controllerName.ToLower(CultureInfo.InvariantCulture), out controller))
                return controller;

            throw new KeyNotFoundException(controllerName);
        }
    }
}
