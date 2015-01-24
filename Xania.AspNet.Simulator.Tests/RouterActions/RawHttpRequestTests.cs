using System.Web.Mvc;
using Moq;
using NUnit.Framework;

namespace Xania.AspNet.Simulator.Tests.RouterActions
{
    public class RawHttpRequestTests
    {
        [TestCase("POST /home/index HTTP/1.1\n", "POST", "/home/index", "HTTP/1.1")]
        [TestCase("GET /home/index HTTP/1.1", "GET", "/home/index", "HTTP/1.1")]
        [TestCase("OPTIONS /home/ HTTP/1.1", "OPTIONS", "/home/", "HTTP/1.1")]
        public void RequestLineTest(string requestLine, string httpMethod, string uriPath, string httpVersion)
        {
            var router = new Mock<Router>().Object;
            var action = router.ParseAction(requestLine);
            Assert.AreEqual(httpMethod, action.HttpMethod);
            Assert.AreEqual(uriPath, action.UriPath);
        }

        [Test]
        public void RouterActionControllerResolveTest()
        {
            // assert
            var router = new Router()
                .RegisterController("home", new HomeController());

            var routerAction = new RouterAction(router)
            {
                HttpMethod = "GET",
                UriPath = "/home/index"
            };

            // act
            var actionResult = routerAction.Execute();

            // assert
            Assert.IsInstanceOf<HomeController>(actionResult.Controller);
        }

        class HomeController: Controller
        {
            public ActionResult Index()
            {
                return null;
            }
        }
    }
}
