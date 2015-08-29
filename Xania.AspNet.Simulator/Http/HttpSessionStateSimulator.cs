using System;
using System.Collections.Generic;
using System.Web;

namespace Xania.AspNet.Simulator.Http
{
    internal class HttpSessionStateSimulator : HttpSessionStateBase
    {
        private readonly string _sessionId;
        private readonly IDictionary<string, object> _values;

        public HttpSessionStateSimulator()
            : this(Guid.NewGuid().ToString("N").ToLowerInvariant())
        {
        }

        public HttpSessionStateSimulator(string sessionId)
        {
            _sessionId = sessionId;
            _values = new Dictionary<string, object>();
        }

        public override void Add(string name, object value)
        {
            _values.Add(name, value);
        }

        public override object this[string name]
        {
            get
            {
                object value;
                return _values.TryGetValue(name, out value) ? value : null;
            }
            set { _values[name] = value; }
        }

        public override string SessionID
        {
            get { return _sessionId; }
        }
    }
}