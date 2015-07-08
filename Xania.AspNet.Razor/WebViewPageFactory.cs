using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc.Razor;
using System.Web.Razor;
using Microsoft.CSharp;
using Xania.AspNet.Core;

namespace Xania.AspNet.Razor
{
    public class WebViewPageFactory
    {
        private readonly IEnumerable<string> _assemblies;

        public WebViewPageFactory(IEnumerable<string> assemblies)
        {
            _assemblies = assemblies;
        }
        
        public IWebViewPage Create(string virtualPath, TextReader reader, DateTime modifiedDateTime)
        {
            var output = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Xania", "AspNet.Razor");
            var cacheFile = new FileInfo(Path.Combine(output, GetCacheKey(virtualPath) + ".dll"));

            Assembly assembly;
            var compilerParameters = GetCompilerParameters();

            if (!compilerParameters.GenerateInMemory && cacheFile.Exists && cacheFile.LastWriteTime > modifiedDateTime)
            {
                Console.WriteLine("load from cache file");
                assembly = Assembly.LoadFrom(cacheFile.FullName);
            }
            else
            {
                Console.WriteLine("compile razor");
                Directory.CreateDirectory(output);

                var generatedCode = GetGeneratedCode(virtualPath, reader);
                assembly = Compile(generatedCode, GetCompilerParameters(), cacheFile);
            }

            var pageType = GetPageType(assembly);
            var instance = (IWebViewPage)Activator.CreateInstance(pageType);
            return instance;
        }

        private static string GetCacheKey(string content)
        {
            using (var md5Hash = MD5.Create())
            {
                return GetMd5Hash(md5Hash, content);
            }
        }

        static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }

        protected virtual Type GetPageType(Assembly assembly)
        {
            var compiledTemplateType = assembly.GetTypes()
                .Single(t => typeof (IWebViewPage).IsAssignableFrom(t));

            return compiledTemplateType;
        }

        protected virtual Assembly Compile(CodeCompileUnit generatedCode, CompilerParameters compilerParameters, FileInfo file)
        {
            var compilerResults = new CSharpCodeProvider().CompileAssemblyFromDom(compilerParameters, generatedCode);

            foreach (CompilerError err in compilerResults.Errors)
            {
                Console.WriteLine(err);
            }

            if (compilerResults.Errors.HasErrors)
            {
                var writer = new StringWriter();
                new CSharpCodeProvider().GenerateCodeFromCompileUnit(generatedCode, writer, new CodeGeneratorOptions {});

                writer.WriteLine("Referenced assemblies: ");
                var q = from string assembly in compilerParameters.ReferencedAssemblies
                    let i = assembly.LastIndexOf('\\') + 1
                    select new {Name = assembly.Substring(i), Path = assembly};

                foreach (var refas in q.OrderBy(e => e.Name))
                {
                    writer.WriteLine("{0} ({1})", refas.Name, refas.Path);
                }

                throw new Exception("Errors in razor file \r\n" + writer);
            }

            if (!compilerParameters.GenerateInMemory)
            {
                File.Copy(compilerResults.PathToAssembly, file.FullName, true);
            }

            return compilerResults.CompiledAssembly;
        }

        private static CodeCompileUnit GetGeneratedCode(string virtualPath, TextReader reader)
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
                    "System.Web.Routing",
                    "Microsoft.Web.WebPages.OAuth"
                }
            };

            var generatedCode =
                new RazorTemplateEngine(host).GenerateCode(reader).GeneratedCode;
            return generatedCode;
        }

        private CompilerParameters GetCompilerParameters()
        {
            var parameters = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false,
                IncludeDebugInformation = false,
                CompilerOptions = "/target:library /optimize",
            };

            parameters.ReferencedAssemblies.AddRange(_assemblies.ToArray());

            return parameters;
        }
    }

}