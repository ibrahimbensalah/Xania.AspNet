using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Mvc;
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
            var controllerContext = action.Execute();
            // assert
            Assert.AreEqual("service is null", controllerContext.Controller.ViewBag.ServiceMessage);
        }

        [Test]
        public void ShouldResolveFilterAttributeDependencies()
        {
            // arrange
            var container = new UnityContainer()
                .RegisterType<IDummyService, DummyService>();
            var action = new HomeController().Action(e => e.Index());
            action.FilterProviders.Add(new UnityFilterAttributeFilterProvider(container));
            var context = action.GetExecutionContext();
            // act
            action.GetActionResult(context);
            // assert
            Assert.AreEqual("service is not null", context.ViewBag.ServiceMessage);
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
