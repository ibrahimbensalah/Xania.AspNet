using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using FluentAssertions;
using NUnit.Framework;

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
            HttpRuntimeHelper.Initialize();
        }

        [TearDown]
        public void StopServer()
        {
            _server.Dispose();
        }

        [TestCase("test/echo/hello", "hello")]
        [TestCase("test/actionusingurl", "/test")]
        [TestCase("test/razorview", "<h1>hello simulator</h1>")]
        public void MvcModuleTest(string path, string content)
        {
            // arrange
            _server.UseMvc(new Router()
                .RegisterController("test", new TestController()));

            using (var client = new HttpClient())
            {
                // act
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
