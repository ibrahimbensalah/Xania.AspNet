using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public static class HttpServerExtensions
    {
        public static void UseMvc(this HttpServerSimulator server, ControllerContainer controllerContainer)
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new ControllerContextViewEngine());

            var mvcApplication = new MvcApplication(controllerContainer);

            server.Use(context =>
            {
                new HttpControllerAction(mvcApplication, context)
                    .Execute()
                    .ExecuteResult();
            });
        }

    }
}
