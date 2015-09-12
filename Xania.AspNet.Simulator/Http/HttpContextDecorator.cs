using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Configuration;
using System.Web.Instrumentation;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.WebSockets;

namespace Xania.AspNet.Simulator.Http
{
    internal class HttpContextDecorator : HttpContextBase
    {
        private readonly HttpContextBase _inner;
        private readonly HttpResponseDecorator _response;

        public HttpContextDecorator(HttpContextBase inner)
        {
            _inner = inner;
            _response = new HttpResponseDecorator(inner.Response);
        }

        public override HttpResponseBase Response
        {
            get { return _response; }
        }

        public override HttpRequestBase Request
        {
            get { return _inner.Request; }
        }

        public override IDictionary Items
        {
            get { return _inner.Items; }
        }

        public override HttpSessionStateBase Session
        {
            get { return _inner.Session; }
        }

        public override IPrincipal User
        {
            get { return _inner.User; }
            set { _inner.User = value; }
        }

        public override Cache Cache
        {
            get { return _inner.Cache; }
        }

        public override PageInstrumentationService PageInstrumentation
        {
            get { return _inner.PageInstrumentation; }
        }

        public override void AcceptWebSocketRequest(Func<AspNetWebSocketContext, Task> userFunc)
        {
            _inner.AcceptWebSocketRequest(userFunc);
        }

        public override bool Equals(object obj)
        {
            return _inner.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _inner.GetHashCode();
        }

        public override void AcceptWebSocketRequest(Func<AspNetWebSocketContext, Task> userFunc,
            AspNetWebSocketOptions options)
        {
            _inner.AcceptWebSocketRequest(userFunc, options);
        }

        public override void AddError(Exception errorInfo)
        {
            _inner.AddError(errorInfo);
        }

        public override ISubscriptionToken AddOnRequestCompleted(Action<HttpContextBase> callback)
        {
            return _inner.AddOnRequestCompleted(callback);
        }

        public override Exception[] AllErrors
        {
            get { return _inner.AllErrors; }
        }

        public override bool AllowAsyncDuringSyncStages
        {
            get { return _inner.AllowAsyncDuringSyncStages; }
            set { _inner.AllowAsyncDuringSyncStages = value; }
        }

        public override HttpApplicationStateBase Application
        {
            get { return _inner.Application; }
        }

        public override HttpApplication ApplicationInstance
        {
            get { return _inner.ApplicationInstance; }
            set { _inner.ApplicationInstance = value; }
        }

        public override AsyncPreloadModeFlags AsyncPreloadMode
        {
            get { return _inner.AsyncPreloadMode; }
            set { _inner.AsyncPreloadMode = value; }
        }

        public override void ClearError()
        {
            _inner.ClearError();
        }

        public override IHttpHandler CurrentHandler
        {
            get { return _inner.CurrentHandler; }
        }

        public override RequestNotification CurrentNotification
        {
            get { return _inner.CurrentNotification; }
        }

        public override ISubscriptionToken DisposeOnPipelineCompleted(IDisposable target)
        {
            return _inner.DisposeOnPipelineCompleted(target);
        }

        public override Exception Error
        {
            get { return _inner.Error; }
        }

        public override object GetGlobalResourceObject(string classKey, string resourceKey)
        {
            return _inner.GetGlobalResourceObject(classKey, resourceKey);
        }

        public override object GetGlobalResourceObject(string classKey, string resourceKey, CultureInfo culture)
        {
            return _inner.GetGlobalResourceObject(classKey, resourceKey, culture);
        }

        public override object GetLocalResourceObject(string virtualPath, string resourceKey)
        {
            return _inner.GetLocalResourceObject(virtualPath, resourceKey);
        }

        public override object GetLocalResourceObject(string virtualPath, string resourceKey, CultureInfo culture)
        {
            return _inner.GetLocalResourceObject(virtualPath, resourceKey, culture);
        }

        public override object GetSection(string sectionName)
        {
            return _inner.GetSection(sectionName);
        }

        public override object GetService(Type serviceType)
        {
            return _inner.GetService(serviceType);
        }

        public override IHttpHandler Handler
        {
            get { return _inner.Handler; }
            set { _inner.Handler = value; }
        }

        public override bool IsCustomErrorEnabled
        {
            get { return _inner.IsCustomErrorEnabled; }
        }

        public override bool IsDebuggingEnabled
        {
            get { return _inner.IsDebuggingEnabled; }
        }

        public override bool IsPostNotification
        {
            get { return _inner.IsPostNotification; }
        }

        public override bool IsWebSocketRequest
        {
            get { return _inner.IsWebSocketRequest; }
        }

        public override bool IsWebSocketRequestUpgrading
        {
            get { return _inner.IsWebSocketRequestUpgrading; }
        }

        public override IHttpHandler PreviousHandler
        {
            get { return _inner.PreviousHandler; }
        }

        public override ProfileBase Profile
        {
            get { return _inner.Profile; }
        }

        public override void RemapHandler(IHttpHandler handler)
        {
            _inner.RemapHandler(handler);
        }

        public override void RewritePath(string path)
        {
            _inner.RewritePath(path);
        }

        public override void RewritePath(string path, bool rebaseClientPath)
        {
            _inner.RewritePath(path, rebaseClientPath);
        }

        public override void RewritePath(string filePath, string pathInfo, string queryString)
        {
            _inner.RewritePath(filePath, pathInfo, queryString);
        }

        public override void RewritePath(string filePath, string pathInfo, string queryString, bool setClientFilePath)
        {
            _inner.RewritePath(filePath, pathInfo, queryString, setClientFilePath);
        }

        public override HttpServerUtilityBase Server
        {
            get { return _inner.Server; }
        }

        public override void SetSessionStateBehavior(SessionStateBehavior sessionStateBehavior)
        {
            _inner.SetSessionStateBehavior(sessionStateBehavior);
        }

        public override bool SkipAuthorization
        {
            get { return _inner.SkipAuthorization; }
            set { _inner.SkipAuthorization = value; }
        }

        public override bool ThreadAbortOnTimeout
        {
            get { return _inner.ThreadAbortOnTimeout; }
            set { _inner.ThreadAbortOnTimeout = value; }
        }

        public override DateTime Timestamp
        {
            get { return _inner.Timestamp; }
        }

        public override string ToString()
        {
            return _inner.ToString();
        }

        public override TraceContext Trace
        {
            get { return _inner.Trace; }
        }

        public override string WebSocketNegotiatedProtocol
        {
            get { return _inner.WebSocketNegotiatedProtocol; }
        }

        public override IList<string> WebSocketRequestedProtocols
        {
            get { return _inner.WebSocketRequestedProtocols; }
        }
    }
}
