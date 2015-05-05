using System;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator.Tests
{
    [SuppressMessage("ReSharper", "Mvc.ViewNotResolved", Justification = "Views are not executed")]
    public class TestController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Hello Simulator!";
            return View();
        }

        [Authorize]
        public ActionResult UserProfile()
        {
            return View();
        }

        [HttpPost]
        public void Update()
        {
        }

        public String ActionUsingUrl()
        {
            return Url.Action("Index");
        }

        public ViewResult RazorView()
        {
            return View("RazorView");
        }

        public string Echo(string id)
        {
            return id;
        }
    }
}