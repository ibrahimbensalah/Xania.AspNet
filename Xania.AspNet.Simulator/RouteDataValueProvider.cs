using System.Globalization;
using System.Web.Mvc;
using System.Web.Routing;

namespace Xania.AspNet.Simulator
{
    public class RouteDataValueProvider : IValueProvider
    {
        private readonly RouteData _routeData;
        private readonly CultureInfo _culture;

        public RouteDataValueProvider(RouteData routeData, CultureInfo culture)
        {
            _routeData = routeData;
            _culture = culture;
        }

        public bool ContainsPrefix(string prefix)
        {
            return _routeData.Values.ContainsKey(prefix);
        }

        public ValueProviderResult GetValue(string key)
        {
            var value = _routeData.Values[key];
            return new ValueProviderResult(value, value.ToString(), _culture);
        }
    }
}