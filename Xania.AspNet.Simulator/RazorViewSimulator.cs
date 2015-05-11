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
    internal class RazorViewSimulator : IView, IDisposable
    {
        private readonly TextReader _contentReader;
        private readonly string _virtualPath;
        private readonly RazorTemplateEngine _engine;
        private readonly CSharpCodeProvider _provider;

        public RazorViewSimulator(TextReader contentReader, string virtualPath)
        {
            _contentReader = contentReader;
            _virtualPath = virtualPath;

            var host = new RazorEngineHost(new CSharpRazorCodeLanguage())
            {
                DefaultBaseClass = typeof (WebViewPage).FullName,
                NamespaceImports =
                {
                    "System" ,
                    "System.Collections.Generic",
                    "System.Linq"
                }
            };

            _engine = new RazorTemplateEngine(host);
            _provider = new CSharpCodeProvider();
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

        public void Render(ViewContext viewContext, TextWriter writer)
        {
            var razorTemplate = _engine.GenerateCode(_contentReader);
            var parameters = CreateComplilerParameters();

            var result = _provider.CompileAssemblyFromDom(parameters, razorTemplate.GeneratedCode);

            foreach (var err in result.Errors)
                Console.WriteLine(err);

            var compiledTemplateType =
                result.CompiledAssembly.GetTypes().SingleOrDefault(t => t.Name == "__CompiledTemplate");

            var instance = (WebViewPage) Activator.CreateInstance(compiledTemplateType);

            RenderView(viewContext, writer, instance);
        }

        private void RenderView(ViewContext viewContext, TextWriter writer, object instance)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            var webViewPage = instance as WebViewPage;
            if (webViewPage == null)
                throw new InvalidOperationException();

            webViewPage.VirtualPath = _virtualPath;
            webViewPage.ViewContext = viewContext;
            webViewPage.ViewData = viewContext.ViewData;
            webViewPage.InitHelpers();

            webViewPage.ExecutePageHierarchy(new WebPageContext(viewContext.HttpContext, (WebPageRenderingBase)null, (object)null), writer, null);
        }

        public void Dispose()
        {
            if (_contentReader != null)
            {
                _contentReader.Close();
                _contentReader.Dispose();
            }
        }
    }
}