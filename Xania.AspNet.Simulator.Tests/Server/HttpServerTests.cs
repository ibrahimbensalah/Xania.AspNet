using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using FluentAssertions;
using NUnit.Framework;
using Xania.AspNet.Razor;

namespace Xania.AspNet.Simulator.Tests.Server
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
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

        [TestCase("test/echo/hello", "hello")]
        [TestCase("test/echo/simulator", "simulator")]
        [TestCase("test/actionusingurl", "/test")]
        [TestCase("test/razorview/1", "<h1>Hello Simulator!</h1>")]
        [TestCase("test/razorview/2", "<h1>Hello Simulator!</h1>")]
        [TestCase("test/razorview/3", "<h1>Hello Simulator!</h1>")]

        [TestCase("test", "<h1>Hello Simulator!</h1>")]
        [TestCase("test/index", "<h1>Hello Simulator!</h1>")]

        [TestCase("test/ViewWithPartial", "<h1>Hello Partial!</h1>")]
        [TestCase("test/ViewWithChildAction", "<h1>Hello ChildAction!</h1>")]
        [TestCase("test/ViewWithLayout", "<h1>Hello ViewWithLayout!</h1>")]
        [TestCase("test/ViewWithModel/model", "<h1>Hello model!</h1>")]
        public void MvcModuleTest(string path, string content)
        {
            // arrange
            var controllerContainer = new ControllerContainer()
                .RegisterController("test", new TestController());

            _server.UseMvc(controllerContainer)
                .EnableRazor();

            using (var client = new HttpClient())
            {
                // act
                var result = client.GetStringAsync(BaseUrl + path).Result
                    .Replace("\r\n", string.Empty)
                    .Replace("\n", string.Empty)
                    .Trim();

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
                for (int i = 0; i < 100; i++)
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

        void Echo(HttpContextBase contextBase)
        {
            var message = contextBase.Request.Params["message"];
            contextBase.Response.Output.Write(message);
        }
    }
}
