Xania.AspNet.Simulator
======================

AspNet Mvc Simulator

[![Build status](https://ci.appveyor.com/api/projects/status/jbiej3ka1n9096nv?svg=true)](https://ci.appveyor.com/project/ibrahimbensalah/xania-aspnet)

[![stable](http://badges.github.io/stability-badges/dist/stable.svg)](http://github.com/badges/stability-badges)

    ...
    using Xania.AspNet.Simulator;
    public class HomeControllerTests
    {
        [Test]
        public void IndexTest()
        {
            var action = new HomeController().Action(c => c.Index());
            
            action.GetAuthorizationResult().Should().BeNull();
            action.ValidateRequest().IsValid.Should().BeTrue();
            action.GetActionResult().Should().BeOfType<ViewResult>();
        }
    }
    public class HomeController: Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
    
