using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Globalization;
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
        public IWebViewPage Create(string virtualPath, TextReader reader)
        {
            var content = reader.ReadToEnd();
            var output = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Xania", "AspNet.Razor");
            var cacheFile = Path.Combine(output, GetCacheKey(content) + ".dll");

            Assembly assembly;

            if (false)
            {
                Console.WriteLine("load from cache file");
                assembly = Assembly.LoadFrom(cacheFile);
            }
            else
            {
                Console.WriteLine("compile razor");
                Directory.CreateDirectory(output);

                var generatedCode = GetGeneratedCode(virtualPath, new StringReader(content));
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

        protected virtual Assembly Compile(CodeCompileUnit generatedCode, CompilerParameters compilerParameters, string output)
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
                foreach (var refas in compilerParameters.ReferencedAssemblies)
                {
                    writer.WriteLine("\t{0}", refas);
                }

                throw new Exception("Errors in razor file \r\n" + writer);
            }
            else
            {
                new CSharpCodeProvider().GenerateCodeFromCompileUnit(generatedCode, Console.Out, new CodeGeneratorOptions { });
            }

            File.Copy(compilerResults.PathToAssembly, output, true);
            
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
                    "System.Web.Routing"
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
                GenerateInMemory = false,
                GenerateExecutable = false,
                IncludeDebugInformation = false,
                CompilerOptions = "/target:library /optimize",
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