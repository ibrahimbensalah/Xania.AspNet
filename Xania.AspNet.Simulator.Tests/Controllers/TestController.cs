using System;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator.Tests.Controllers
{
    [SuppressMessage("ReSharper", "Mvc.ViewNotResolved", Justification = "Views are not executed")]
    [SuppressMessage("ReSharper", "Mvc.PartialViewNotResolved")]
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

        public String ActionUsingSession(string id)
        {
            var value = Session[id];
            return value == null ? null : value.ToString();
        }

        public ViewResult RazorView()
        {
            return View();
        }

        public string Echo(string id)
        {
            return id;
        }

        public string EchoJson(JsonInput input)
        {
            return input.Value;
        }

        public string Query(string q)
        {
            return q;
        }

        public ViewResult ViewWithPartial()
        {
            return View();
        }

        public ViewResult ViewWithChildAction()
        {
            return View();
        }
        public ViewResult ViewWithLayout()
        {
            return View();
        }

        public ViewResult ViewWithModel(string id)
        {
            return View(model: id);
        }

        [ChildActionOnly]
        public ActionResult ChildPartialViewAction()
        {
            return PartialView();
        }
    }

    public class JsonInput
    {
        public string Value { get; set; }
    }
}