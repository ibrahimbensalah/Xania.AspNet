using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Xania.AspNet.Simulator
{
    public class SimulatorValueProvider : IValueProvider
    {
        private readonly ControllerContext _controllerContext;
        private readonly CultureInfo _culture;

        public SimulatorValueProvider(ControllerContext controllerContext, CultureInfo culture)
        {
            _controllerContext = controllerContext;
            _culture = culture;
        }

        public bool ContainsPrefix(string prefix)
        {
            return _controllerContext.RouteData.Values.ContainsKey(prefix) ||
                   _controllerContext.HttpContext.Request.Form.AllKeys.Contains(prefix) ||
                   _controllerContext.HttpContext.Request.QueryString.AllKeys.Contains(prefix);
        }

        public ValueProviderResult GetValue(string key)
        {
            var value = _controllerContext.RouteData.Values[key] ??
                _controllerContext.HttpContext.Request.Form[key] ??
                _controllerContext.HttpContext.Request.QueryString[key];

            if (value == null)
                return null;

            return new ValueProviderResult(value, value.ToString(), _culture);
        }
    }
}