using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace Xania.AspNet.Simulator
{
    public static class HttpServerExtensions
    {
        public static void UseMvc(this HttpServerSimulator server, ControllerContainer controllerContainer)
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new ControllerContextViewEngine());

            DisplayModeProvider.Instance.Modes.Clear();

            var mvcApplication = new MvcApplication(controllerContainer);

            server.Use(context =>
            {
                var action = new HttpControllerAction(mvcApplication, context)
                    .Execute();

                if (action == null)
                    throw new HttpException(404, context.Request.Url.ToString());

                action.ExecuteResult();
            });
        }

    }
}
