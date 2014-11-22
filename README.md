Xania.AspNet.Simulator
======================

AspNet Mvc Simulator

    var sim = new MvcSimulator();
    var request = sim.CreateRequest((HomeController c) => c.Index());
    var actionResult = (ViewResult)request.Execute();
    Assert.AreEqual("Home", actionResult.ViewBag.Title);
