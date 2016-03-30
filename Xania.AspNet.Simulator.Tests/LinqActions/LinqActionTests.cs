using System.Web.Mvc;
using FluentAssertions;
using NUnit.Framework;
using Xania.AspNet.Simulator.Tests.Controllers;

namespace Xania.AspNet.Simulator.Tests.LinqActions
{
    public class LinqActionTests: ControllerActionBaseTests
    {
        [Test]
        public void PartialViewResultTest()
        {
            var action = new TestController()
                .ChildAction(c => c.ChildPartialViewAction());

            action.GetAuthorizationResult().Should().BeNull();
            var result = action.GetActionResult();

            result.Should().BeOfType<PartialViewResult>();
        }

        protected override ControllerAction GetIndexAction(TestController testController)
        {
            return testController.Action(c => c.Index());
        }

        protected override ControllerAction GetUpdateAction(TestController testController)
        {
            return testController.Action(c => c.Update());
        }

        protected override ControllerAction GetIndexAction(HomeController homeController)
        {
            return homeController.Action(c => c.Index());
        }

        protected override ControllerAction GetAboutAction(HomeController homeController)
        {
            return homeController.Action(c => c.About());
        }

        protected override ControllerAction GetIndexAction(AdminController adminController)
        {
            return adminController.Action(c => c.Index());
        }

        protected override ControllerAction GetPublicAction(HomeController homeController)
        {
            return homeController.Action(c => c.Public());
        }

        protected override ControllerAction GetChildAction(TestController testController)
        {
            return testController.Action(c => c.ChildPartialViewAction());
        }

        protected override ControllerAction GetActionUsingUrl(TestController testController)
        {
            return testController.Action(c => c.ActionUsingUrl());
        }
    }
}
