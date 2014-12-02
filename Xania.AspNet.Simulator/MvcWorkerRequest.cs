using System;
using System.IO;
using System.Security.Principal;
using System.Web;
using System.Web.Hosting;

namespace Xania.AspNet.Simulator
{
    internal class MvcWorkerRequest: SimpleWorkerRequest
    {
        private readonly string _httpMethod;
        private readonly IPrincipal _user;

        public MvcWorkerRequest(string url, string httpMethod, IPrincipal user)
            : base("", @"/simulator", url.Substring(1), String.Empty, new StreamWriter(new MemoryStream()))
        {
            _httpMethod = httpMethod;
            _user = user;
        }

        public override string GetHttpVerbName()
        {
            return _httpMethod;
        }
    }
}
