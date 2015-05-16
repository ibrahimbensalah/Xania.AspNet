using System.Web.Mvc;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace Xania.AspNet.Simulator.Tests.LinqActions
{
    public class LinqActionFilterProvidersTests
    {
        [Test]
        public void WhenUnableToResolveThenShouldContinueSilentlyTest()
        {
            // arrange
            var action = new HomeController().Action(e => e.Index());
            // act
            var result = action.Execute();
            // assert
            Assert.AreEqual("service is null", result.ViewBag.ServiceMessage);
        }

        [Test]
        public void ShouldResolveFilterAttributeDependencies()
        {
            // arrange
            var container = new UnityContainer()
                .RegisterType<IDummyService, DummyService>();
            var action = new HomeController().Action(e => e.Index());
            action.Resolve = type => container.Resolve(type);
            // act
            var result = action.Execute();
            // assert
            Assert.AreEqual("service is not null", result.ViewBag.ServiceMessage);
        }

        class HomeController: Controller
        {
            [DependentActionFilter]
            public ActionResult Index()
            {
                return null;
            }
        }

        class DependentActionFilter : ActionFilterAttribute
        {
            public override void OnActionExecuted(ActionExecutedContext filterContext)
            {
                filterContext.Controller.ViewBag.ServiceMessage = Service == null
                    ? "service is null"
                    : "service is not null";
            }

            [Dependency]
            public IDummyService Service { get; set; }

            public bool ThrowWhenMissing { get; set; }
        }

        class DummyService : IDummyService { }

        private interface IDummyService
        {
        }
    }
}
