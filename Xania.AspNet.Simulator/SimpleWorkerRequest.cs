using System;
using System.Linq;
using System.Web;

namespace Xania.AspNet.Simulator
{
    public class SimpleWorkerRequest : HttpWorkerRequest
    {
        private readonly HttpRequestInfo _requestInfo;

        public SimpleWorkerRequest(HttpRequestInfo requestInfo, string httpVersion = null)
        {
            _requestInfo = requestInfo;
        }

        public override string GetUriPath()
        {
            return _requestInfo.UriPath;
        }

        public override string GetQueryString()
        {
            return String.Empty;
        }

        public override string GetRawUrl()
        {
            return GetUriPath();
        }

        public override string GetHttpVerbName()
        {
            return _requestInfo.HttpMethod;
        }

        public override string GetHttpVersion()
        {
            return _requestInfo.HttpVersion ?? "HTTP/1.1";
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
            return "127.0.0.1";
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
