using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xania.AspNet.Simulator.Tests
{
    class TestHelper
    {
        public static string MvcApplication1
        {
            get
            {
                var solutionDir = new PropertiesFile("xania.properties").Get("SolutionDir");
                return Path.Combine(solutionDir, "MvcApplication1");
            }
        }
        public static string SimulatorTests
        {
            get
            {
                var solutionDir = new PropertiesFile("xania.properties").Get("SolutionDir");
                return Path.Combine(solutionDir, "Xania.AspNet.Simulator.Tests");
            }
        }
    }
}
