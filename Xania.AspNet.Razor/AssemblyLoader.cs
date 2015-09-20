using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Xania.AspNet.Razor
{
    internal class AssemblyLoader
    {
        private static readonly IDictionary<string, Assembly> Assemblies;

        static AssemblyLoader()
        {
            Assemblies = new Dictionary<string, Assembly>(StringComparer.InvariantCultureIgnoreCase);
        }

        public static Assembly GetAssembly(string assemblyFile)
        {
            var key = Path.GetFullPath(assemblyFile);
            Assembly result;
            if (!Assemblies.TryGetValue(key, out result))
            {
                result = Assembly.LoadFile(key);
                Assemblies.Add(key, result);
            }
            return result;
        }
    }
}
