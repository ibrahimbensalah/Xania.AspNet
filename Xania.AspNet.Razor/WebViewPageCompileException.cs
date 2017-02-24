using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Text;

namespace Xania.AspNet.Razor
{
    public sealed class WebViewPageCompileException : Exception
    {
        public WebViewPageCompileException(CompilerResults compilerResults, string source)
            : base(GetMessage(compilerResults))
        {
            CompilerResults = compilerResults;
            Source = source;
        }
        public CompilerParameters CompilerParameters { get; set; }
        public CodeCompileUnit CompileUnit { get; set; }
        public CompilerResults CompilerResults { get; set; }

        private static string GetMessage(CompilerResults compilerResults)
        {
            var sb = new StringBuilder();
            foreach (CompilerError err in compilerResults.Errors)
            {
                sb.AppendFormat("({0}, {1}) : {2}", err.Line, err.Column, err.ErrorText);
            }
            return sb.ToString();
        }
    }
}