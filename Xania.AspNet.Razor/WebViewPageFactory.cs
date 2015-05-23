using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc.Razor;
using System.Web.Razor;
using System.Web.WebPages;
using Microsoft.CSharp;
using Xania.AspNet.Core;

namespace Xania.AspNet.Razor
{
    public class WebViewPageFactory
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

}