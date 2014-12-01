using System;
using System.IO;
using System.Web;

namespace Xania.AspNet.Simulator
{
    internal class SimpleWorkerRequest : HttpWorkerRequest
    {
        public SimpleWorkerRequest()
        {
        }

        public string HttpMethod { get; set; }

        public override string GetUriPath()
        {
            throw new NotImplementedException();
        }

        public override string GetQueryString()
        {
            throw new NotImplementedException();
        }

        public override string GetRawUrl()
        {
            throw new NotImplementedException();
        }

        public override string GetHttpVerbName()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public override int GetLocalPort()
        {
            throw new NotImplementedException();
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
