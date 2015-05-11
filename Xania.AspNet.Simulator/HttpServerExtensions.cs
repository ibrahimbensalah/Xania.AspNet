using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public static class HttpServerExtensions
    {
        public static void UseMvc(this HttpServerSimulator server, ControllerContainer controllerContainer)
        {
            server.Use(context =>
            {
                if (context.Request.Url == null) 
                    return;

                var path = context.Request.Url.AbsolutePath;
                var action = new RouterAction(controllerContainer)
                {
                    UriPath = path
                };
                var result = action.Execute(context);
                result.ExecuteResult();
            });
        }
    }
}
