using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using FluentAssertions;
using NUnit.Framework;
using Xania.AspNet.Razor;
using Xania.AspNet.Simulator.Tests.Controllers;

namespace Xania.AspNet.Simulator.Tests.Server
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class HttpServerUnitTests : HttpServerTestBase
    {
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
            var contentProvider = SystemUnderTest.GetSimulatorTestsContentProvider();
            Server.UseStatic(contentProvider);

            Server.UseMvc(new TestController(), contentProvider)
                .EnableRazor();

            using (var client = new HttpClient())
            {
                // act
                var result = client.GetStringAsync(GetUrl(path)).Result
                    .Replace("\r\n", string.Empty)
                    .Replace("\n", string.Empty)
                    .Trim();

                // assert
                result.Should().Be(content);
            }
        }

        [TestCase("app.config")]
        [TestCase("packages.config")]
        [TestCase("Views/_ViewStart.cshtml")]
        public void StaticModuleTest(string path)
        {
            // arrange
            var contentProvider = SystemUnderTest.GetSimulatorTestsContentProvider();
            Server.UseStatic(contentProvider);

            using (var client = new HttpClient())
            {
                // act
                var result = client.GetByteArrayAsync(GetUrl(path)).Result;

                // assert
                var mem = new MemoryStream();
                contentProvider.Open(path).CopyTo(mem);
                var content = mem.ToArray();
                Assert.AreEqual(content, result);
            }
        }

        [Test]
        public void SimultanousRequestsTest()
        {
            Server.Use(Echo);

            using (var client = new HttpClient())
            {
                var tasks = new List<Task>();
                for (int i = 0; i < 100; i++)
                {
                    var message = "msg-" + i;
                    tasks.Add(client.GetStringAsync(GetUrl("") + "?message=" + message).ContinueWith(t =>
                    {
                        t.Result.Should().Be(message);
                    }));
                }
                Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(10));
            }
        }

        bool Echo(HttpContextBase contextBase)
        {
            var message = contextBase.Request.Params["message"];
            contextBase.Response.Output.Write(message);

            return true;
        }
    }
}
