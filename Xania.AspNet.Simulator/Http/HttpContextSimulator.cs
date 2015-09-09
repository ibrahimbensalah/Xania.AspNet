using System.Web;

namespace Xania.AspNet.Simulator.Http
{
    internal class HttpContextSimulator : HttpContextWrapper
    {
        private readonly HttpRequestSimulator _request;
        private readonly HttpResponseWrapper _response;
        private readonly HttpSessionStateSimulator _session;

        public HttpContextSimulator(HttpContext httpContext) : base(httpContext)
        {
            _request = new HttpRequestSimulator(httpContext.Request);
            _response = new HttpResponseWrapper(httpContext.Response);
            _session = new HttpSessionStateSimulator();
        }

        public override HttpRequestBase Request
        {
            get { return _request; }
        }

        public override HttpResponseBase Response
        {
            get { return _response; }
        }

        public override HttpSessionStateBase Session
        {
            get { return _session; }
        }
    }

    internal class HttpRequestSimulator : HttpRequestWrapper
    {
        public HttpRequestSimulator(HttpRequest httpRequest)
            : base(httpRequest)
        {
        }

        public override HttpBrowserCapabilitiesBase Browser
        {
            get { return new HttpBrowserCapabilitiesSimulator(); }
        }

        public override string AppRelativeCurrentExecutionFilePath
        {
            get { return "~" + Url.AbsolutePath; }
        }
    }
}