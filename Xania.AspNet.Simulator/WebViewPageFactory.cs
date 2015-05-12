using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Razor;
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

        public WebViewPage Create(string virtualPath)
        {
            using (var stream = _contentProvider.Open(virtualPath))
            {
                var _contentReader = new StreamReader(stream);
                var host = new RazorEngineHost(new CSharpRazorCodeLanguage())
                {
                    DefaultBaseClass = typeof (WebViewPage).FullName,
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

                return (WebViewPage) Activator.CreateInstance(compiledTemplateType);
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
}