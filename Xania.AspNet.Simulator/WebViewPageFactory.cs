using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Razor;
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

        public IWebViewPage Create(string relativePath)
        {
            using (var stream = _contentProvider.Open(relativePath))
            {
                var host = new MvcWebPageRazorHost("~/Views/Test/Index.cshtml", @"C:\Development\GitHub\asdfasdf.cshtml")
                {
                    DefaultBaseClass = typeof(WebViewPageSimulator<>).FullName.TrimEnd('`', '1'),
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

                var generatedCode =
                    new RazorTemplateEngine(host).GenerateCode(new StreamReader(stream)).GeneratedCode;

                var compilerResults = new CSharpCodeProvider().CompileAssemblyFromDom(GetCompilerParameters(), generatedCode);

                foreach (CompilerError err in compilerResults.Errors)
                {
                    Console.WriteLine(err);
                }

                if (compilerResults.Errors.HasErrors)
                {
                    var writer = new StringWriter();
                    new CSharpCodeProvider().GenerateCodeFromCompileUnit(generatedCode, writer, new CodeGeneratorOptions{});
                    throw new Exception("Errors in razor file \r\n" + writer);
                }

                var compiledTemplateType =
                    compilerResults.CompiledAssembly.GetTypes().SingleOrDefault(t => t.Name == host.DefaultClassName);

                return (IWebViewPage) Activator.CreateInstance(compiledTemplateType);
            }
        }

        private CompilerParameters GetCompilerParameters()
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

    public interface IWebViewPage
    {
        void Initialize(ViewContext viewContext, string virtualPath, IMvcApplication mvcApplication);
        void Execute(HttpContextBase httpContext, TextWriter writer);
    }

    public abstract class WebViewPageSimulator<TModel> : WebViewPage<TModel>, IWebViewPage
    {
        public new HtmlHelperSimulator<TModel> Html { get; set; }

        public virtual void Initialize(ViewContext viewContext, string virtualPath, IMvcApplication mvcApplication)
        {
            VirtualPath = virtualPath;
            ViewContext = viewContext;
            ViewData = new ViewDataDictionary<TModel>(viewContext.ViewData);

            Ajax = new AjaxHelper<TModel>(viewContext, this, mvcApplication.Routes);
            Html = new HtmlHelperSimulator<TModel>(viewContext, this, mvcApplication);
            Url = new UrlHelper(viewContext.RequestContext, mvcApplication.Routes);
            VirtualPathFactory = mvcApplication;

        }

        public void Execute(HttpContextBase httpContext, TextWriter writer)
        {
            ExecutePageHierarchy(new WebPageContext(httpContext, null, null), writer, null);
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