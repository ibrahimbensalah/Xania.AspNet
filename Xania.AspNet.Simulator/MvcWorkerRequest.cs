using System;
using System.IO;
using System.Security.Principal;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    internal class MvcWorkerRequest : HttpWorkerRequest
    {
        private readonly string _url;
        private readonly string _httpMethod;

        public MvcWorkerRequest(string url, string httpMethod)
        {
            _url = url;
            _httpMethod = httpMethod;
        }

        public override string GetUriPath()
        {
            return "/";
        }

        public override string GetQueryString()
        {
            return String.Empty;
        }

        public override string GetRawUrl()
        {
            return String.Format("http://localhost:80{0}", _url);
        }

        public override string GetHttpVerbName()
        {
            return _httpMethod;
        }

        public override string GetHttpVersion()
        {
            throw new NotImplementedException();
        }

        public override string GetRemoteAddress()
        {
            throw new NotImplementedException();
        }

        public override int GetRemotePort()
        {
            throw new NotImplementedException();
        }

        public override string GetLocalAddress()
        {
            return "http://localhost";
        }

        public override int GetLocalPort()
        {
            return 80;
        }

        public override void SendStatus(int statusCode, string statusDescription)
        {
            throw new NotImplementedException();
        }

        public override void SendKnownResponseHeader(int index, string value)
        {
            throw new NotImplementedException();
        }

        public override void SendUnknownResponseHeader(string name, string value)
        {
            throw new NotImplementedException();
        }

        public override void SendResponseFromMemory(byte[] data, int length)
        {
            throw new NotImplementedException();
        }

        public override void SendResponseFromFile(string filename, long offset, long length)
        {
            throw new NotImplementedException();
        }

        public override void SendResponseFromFile(IntPtr handle, long offset, long length)
        {
            throw new NotImplementedException();
        }

        public override void FlushResponse(bool finalFlush)
        {
            throw new NotImplementedException();
        }

        public override void EndOfRequest()
        {
            throw new NotImplementedException();
        }
    }
}
