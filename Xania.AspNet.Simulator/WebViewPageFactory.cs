using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Razor;
using System.Web.WebPages;
using Microsoft.CSharp;

namespace Xania.AspNet.Simulator
{
    internal class WebViewPageFactory
    {
        private readonly IContentProvider _contentProvider;

        public WebViewPageFactory(IContentProvider contentProvider)
        {
            _contentProvider = contentProvider;
        }

        public WebViewPageSimulator Create(string relativePath)
        {
            using (var stream = _contentProvider.Open(relativePath))
            {
                var _contentReader = new StreamReader(stream);
                var host = new RazorEngineHost(new CSharpRazorCodeLanguage())
                {
                    DefaultBaseClass = typeof (WebViewPageSimulator).FullName,
                    NamespaceImports =
                    {
                        "System",
                        "System.Collections.Generic",
                        "System.Linq",
                        "System.Web.Mvc",
                        "System.Web.Mvc.Ajax",
                        "System.Web.Mvc.Html",
                        "System.Web.Routing"
                    }
                };

                var engine = new RazorTemplateEngine(host);
                var provider = new CSharpCodeProvider();

                var razorTemplate = engine.GenerateCode(_contentReader);
                var parameters = CreateComplilerParameters();

                var result = provider.CompileAssemblyFromDom(parameters, razorTemplate.GeneratedCode);

                foreach (var err in result.Errors)
                    Console.WriteLine(err);

                var compiledTemplateType =
                    result.CompiledAssembly.GetTypes().SingleOrDefault(t => t.Name == "__CompiledTemplate");

                return (WebViewPageSimulator) Activator.CreateInstance(compiledTemplateType);
            }
        }

        private CompilerParameters CreateComplilerParameters()
        {
            var parameters = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false,
                IncludeDebugInformation = false,
                CompilerOptions = "/target:library /optimize"
            };

            var assemblies = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => !a.IsDynamic)
                .GroupBy(a => a.FullName)
                .Select(grp => grp.First())
                .Select(a => a.Location)
                .Where(a => !String.IsNullOrWhiteSpace(a))
                .ToArray();

            parameters.ReferencedAssemblies.AddRange(assemblies);

            return parameters;
        }
    }

    public abstract class WebViewPageSimulator: WebViewPage
    {
        public new HtmlHelperSimulator<object> Html { get; set; }

        public override string NormalizePath(string path)
        {
            return path;
        }
    }

    public class HtmlHelperSimulator<T> : HtmlHelper<T>
    {
        private readonly IMvcApplication _mvcApplication;

        internal HtmlHelperSimulator(ViewContext viewContext, IViewDataContainer viewDataContainer, IMvcApplication mvcApplication) 
            : base(viewContext, viewDataContainer, mvcApplication.Routes)
        {
            _mvcApplication = mvcApplication;
        }

        public MvcHtmlString Action(string actionName, object routeValues)
        {
            var controllerName = ViewContext.RouteData.GetRequiredString("controller");
            var action = _mvcApplication.Action(controllerName, actionName);
            action.Data(routeValues);

            action.Execute().ExecuteResult();
            return MvcHtmlString.Create(action.Output.ToString());
        }
    }
}