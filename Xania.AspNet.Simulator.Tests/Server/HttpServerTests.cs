using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using FluentAssertions;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Xania.AspNet.Simulator.Tests.LinqActions;

namespace Xania.AspNet.Simulator.Tests.Server
{
    public class HttpServerTests
    {
        const string BaseUrl = "http://localhost:9989/";
        private HttpServerSimulator _server;

        [SetUp]
        public void StartServer()
        {
            _server = new HttpServerSimulator(BaseUrl);
        }

        [TearDown]
        public void StopServer()
        {
            _server.Dispose();
        }

        [TestCase("test/ActionUsingUrl", "/test")]
        [TestCase("test/echo/hello", "hello")]
        public void MvcModuleTest(string path, string content)
        {
            // arrange
            _server.UseMvc(new Router()
                .RegisterController("test", new TestController()));
            // act
            using (var client = new HttpClient())
            {
                var result = client.GetStringAsync(BaseUrl + path).Result;

                // assert
                result.Should().Be(content);
            }
        }

        [Test]
        public void SimultanousRequestsTest()
        {
            _server.Use(Echo);

            using (var client = new HttpClient())
            {
                var tasks = new List<Task>();
                for (int i = 0; i < 1; i++)
                {
                    var message = "msg-" + i;
                    tasks.Add(client.GetStringAsync(BaseUrl + "?message=" + message).ContinueWith(t =>
                    {
                        t.Result.Should().Be(message);
                    }));
                }
                Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(10));
            }
        }

        [Test]


        void Echo(HttpContextBase contextBase)
        {
            var message = contextBase.Request.Params["message"];
            contextBase.Response.Output.Write(message);
        }
    }
}
