namespace Xania.AspNet.Simulator
{
    public static class HttpServerExtensions
    {
        public static void UseMvc(this HttpServerSimulator server, Router router)
        {
            server.Use(context =>
            {
                if (context.Request.Url != null)
                {
                    var path = context.Request.Url.AbsolutePath;
                    var action = new RouterAction(router) { UriPath = path };
                    var result = action.Execute(context);
                    result.ExecuteResult();
                }
            });
        }
    }
}
