using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Xania.AspNet.Simulator.Tests.Server
{
    public class HttpServerTestBase
    {
        private int _port = 4040;

        protected string BaseUrl;
        protected HttpServerSimulator Server { get; private set; }

        [SetUp]
        public virtual void StartServer()
        {
            _port ++;
            BaseUrl = String.Format("http://localhost:{0}/", _port);
            Server = new HttpServerSimulator(BaseUrl);
        }

        [TearDown]
        public virtual void StopServer()
        {
            Server.Dispose();
        }

    }
}
