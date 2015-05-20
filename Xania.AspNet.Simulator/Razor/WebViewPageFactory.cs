using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Razor;
using System.Web.Razor;
using System.Web.WebPages;
using Microsoft.CSharp;

namespace Xania.AspNet.Simulator.Razor
{
    internal class WebViewPageFactory
    {
        public IWebViewPage Create(string virtualPath, TextReader reader)
        {
            var host = new MvcWebPageRazorHost(virtualPath, string.Empty)
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

            var generatedCode =
                new RazorTemplateEngine(host).GenerateCode(reader).GeneratedCode;

            var compilerResults = new CSharpCodeProvider().CompileAssemblyFromDom(GetCompilerParameters(), generatedCode);

            foreach (CompilerError err in compilerResults.Errors)
            {
                Console.WriteLine(err);
            }

            if (compilerResults.Errors.HasErrors)
            {
                var writer = new StringWriter();
                new CSharpCodeProvider().GenerateCodeFromCompileUnit(generatedCode, writer, new CodeGeneratorOptions {});
                throw new Exception("Errors in razor file \r\n" + writer);
            }

            var compiledTemplateType =
                compilerResults.CompiledAssembly.GetTypes().SingleOrDefault(t => t.Name == host.DefaultClassName);

            return (IWebViewPage) Activator.CreateInstance(compiledTemplateType);
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

    public class StartBaseSimulator : WebPageRenderingBase
    {
        public override void Execute()
        {
            throw new NotImplementedException();
        }

        public override void Write(HelperResult result)
        {
            throw new NotImplementedException();
        }

        public override void Write(object value)
        {
            throw new NotImplementedException();
        }

        public override void WriteLiteral(object value)
        {
            throw new NotImplementedException();
        }

        public override void ExecutePageHierarchy()
        {
        }

        public override HelperResult RenderPage(string path, params object[] data)
        {
            throw new NotImplementedException();
        }

        public override string Layout
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override IDictionary<object, dynamic> PageData
        {
            get { throw new NotImplementedException(); }
        }

        public override dynamic Page
        {
            get { throw new NotImplementedException(); }
        }
    }

    public class VirtualPathFactorySimulator : IVirtualPathFactory
    {
        private readonly IMvcApplication _mvcApplication;

        public VirtualPathFactorySimulator(IMvcApplication mvcApplication)
        {
            _mvcApplication = mvcApplication;
        }

        public bool Exists(string virtualPath)
        {
            return _mvcApplication.Exists(virtualPath);
        }

        public object CreateInstance(string virtualPath)
        {
            return _mvcApplication.Create(virtualPath);
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