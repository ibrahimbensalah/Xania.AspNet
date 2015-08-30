using System.IO;
using FluentAssertions;
using NSubstitute;
using Xania.AspNet.Simulator.Tests.Controllers;

namespace Xania.AspNet.Simulator.Tests.NamedActions
{
    public class NamedActionTests: ControllerActionBaseTests
    {
        protected override ControllerAction GetIndexAction(TestController testController)
        {
            return testController.Action("index");
        }

        protected override ControllerAction GetUpdateAction(TestController testController)
        {
            return testController.Action("update");
        }

        protected override ControllerAction GetIndexAction(HomeController homeController)
        {
            return homeController.Action("index");
        }

        protected override ControllerAction GetAboutAction(HomeController homeController)
        {
            return homeController.Action("about");
        }

        protected override ControllerAction GetIndexAction(AdminController adminController)
        {
            return adminController.Action("index");
        }

        protected override ControllerAction GetPublicAction(HomeController homeController)
        {
            return homeController.Action("public");
        }

        protected override ControllerAction GetChildAction(TestController testController)
        {
            return testController.Action("ChildPartialViewAction");
        }

        protected override ControllerAction GetActionUsingUrl(TestController testController)
        {
            return testController.Action("ActionUsingUrl");
        }
    }
}
