Xania.AspNet.Simulator
======================

AspNet Mvc Simulator

	...
	using Xania.AspNet.Simulator;

    public class HomeControllerTests
    {
        [Test]
        public void IndexTest()
        {
            var result = new HomeController().Execute(c => c.Index());
            Assert.AreEqual("Hello Simulator!", result.ViewBag.Message);
        }
	}

	public class HomeController: Controller
	{
		public ActionResult Index()
		{
			ViewBag.Message = "Hello Simulator!";
			return View();
		}
	}
