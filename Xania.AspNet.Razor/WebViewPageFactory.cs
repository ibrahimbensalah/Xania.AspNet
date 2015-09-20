using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class WebViewPageFactory: IWebViewPageFactory
    {
        private readonly IEnumerable<string> _assemblyFiles;
        private readonly IEnumerable<string> _namespaces;
        private readonly bool _cacheEnabled;

        public WebViewPageFactory(IEnumerable<string> assemblyFiles, IEnumerable<string> namespaces)
        {
            _assemblyFiles = assemblyFiles;
            _namespaces = namespaces;
            _cacheEnabled = true;
        }

        public IWebViewPage Create(string virtualPath, TextReader reader, DateTime modifiedDateTime)
        {
            var output = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Xania.AspNet");
            var cacheFile = new FileInfo(Path.Combine(output, GetCacheKey(virtualPath) + ".dll"));

            Assembly assembly;
            if (_cacheEnabled && cacheFile.Exists && cacheFile.LastWriteTime > modifiedDateTime)
            {
                assembly = AssemblyLoader.GetAssembly(cacheFile.FullName);
            }
            else
            {
                Directory.CreateDirectory(output);

                var generatedCode = GetGeneratedCode(virtualPath, reader);

                // new CSharpCodeProvider().GenerateCodeFromCompileUnit(generatedCode, Console.Out, new CodeGeneratorOptions());

                assembly = Compile(generatedCode, GetCompilerParameters(), cacheFile);
            }

            var pageType = GetPageType(assembly);
            var instance = (IWebViewPage)Activator.CreateInstance(pageType);
            return instance;
        }

        private string GetCacheKey(string content)
        {
            var versions = from assemblyPath in _assemblyFiles
                let myFileVersionInfo = FileVersionInfo.GetVersionInfo(assemblyPath)
                orderby myFileVersionInfo.FileVersion
                select myFileVersionInfo.FileVersion;

            using (var md5Hash = MD5.Create())
            {
                return GetMd5Hash(md5Hash, content + string.Join(",", versions));
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

            if (!compilerParameters.GenerateInMemory && _cacheEnabled)
            {
                File.Copy(compilerResults.PathToAssembly, file.FullName, true);
            }

            return compilerResults.CompiledAssembly;
        }

        private CodeCompileUnit GetGeneratedCode(string virtualPath, TextReader reader)
        {
            var host = new MvcWebPageRazorHost(virtualPath, string.Empty)
            {
                DefaultBaseClass = typeof (WebViewPageSimulator).FullName
            };

            foreach(var ns in _namespaces)
                host.NamespaceImports.Add(ns);

            var generatedCode =
                new RazorTemplateEngine(host).GenerateCode(reader).GeneratedCode;
            return generatedCode;
        }

        private CompilerParameters GetCompilerParameters()
        {
            var parameters = new CompilerParameters
            {
                GenerateInMemory = !_cacheEnabled,
                GenerateExecutable = false,
                IncludeDebugInformation = false,
                CompilerOptions = "/target:library",
            };

            parameters.ReferencedAssemblies.AddRange(_assemblyFiles.ToArray());

            return parameters;
        }
    }

}