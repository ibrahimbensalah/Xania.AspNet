using System.IO;
using FluentAssertions;
using NSubstitute;
using Xania.AspNet.Simulator.Tests.Controllers;

namespace Xania.AspNet.Simulator.Tests.NamedActions
{
    public class AreaNamedActionTests : ControllerActionBaseTests
    {
        protected override ControllerAction GetIndexAction(TestController testController)
        {
            return testController.Action("area51/index");
        }

        protected override ControllerAction GetUpdateAction(TestController testController)
        {
            return testController.Action("area51/update");
        }

        protected override ControllerAction GetIndexAction(HomeController homeController)
        {
            return homeController.Action("area51/index");
        }

        protected override ControllerAction GetAboutAction(HomeController homeController)
        {
            return homeController.Action("area51/about");
        }

        protected override ControllerAction GetIndexAction(AdminController adminController)
        {
            return adminController.Action("area51/index");
        }

        protected override ControllerAction GetPublicAction(HomeController homeController)
        {
            return homeController.Action("area51/public");
        }

        protected override ControllerAction GetChildAction(TestController testController)
        {
            return testController.Action("area51/ChildPartialViewAction");
        }

        protected override ControllerAction GetActionUsingUrl(TestController testController)
        {
            return testController.Action("area51/ActionUsingUrl");
        }
    }
}
