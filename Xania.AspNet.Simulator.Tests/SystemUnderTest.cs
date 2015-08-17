using System.IO;

namespace Xania.AspNet.Simulator.Tests
{
    public class SystemUnderTest
    {
        public static DirectoryContentProvider GetMvcApp1ContentProvider()
        {
            return new DirectoryContentProvider(Path.Combine(SolutionDir, "MvcApplication1"));
        }

        public static DirectoryContentProvider GetSimulatorTestsContentProvider()
        {
            return new DirectoryContentProvider(Path.Combine(SolutionDir, "Xania.AspNet.Simulator.Tests"));
        }

        private static string SolutionDir
        {
            get
            {
                return new PropertiesFile("xania.properties").Get("SolutionDir");
            }
        }
    }
}
