using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Web.Compilation;
using System.Web.Hosting;

namespace Xania.AspNet.Simulator
{
    public class HttpRuntimeHelper
    {
        private static BuildManager _buildManager;
        private static bool _initialized;
        private static Type VirtualPathType { get; set; }

        internal const string BinDirectoryName = "bin";
        internal const string CodeDirectoryName = "App_Code";
        internal const string WebRefDirectoryName = "App_WebReferences";
        internal const string ResourcesDirectoryName = "App_GlobalResources";
        internal const string LocalResourcesDirectoryName = "App_LocalResources";
        internal const string DataDirectoryName = "App_Data";
        internal const string ThemesDirectoryName = "App_Themes";
        internal const string GlobalThemesDirectoryName = "Themes";
        internal const string BrowsersDirectoryName = "App_Browsers";

        public const string AppDomainAppPath = @"C:\Dev\Xania.AspNet-master\Xania.AspNet.Simulator.Tests\Server";

        static HttpRuntimeHelper()
        {
            VirtualPathType = typeof(HttpRuntime).Assembly.GetTypes()
                .First(t => t.Name == "VirtualPath");

            SetPrivateField(CurrentBuildManager, "_excludedTopLevelDirectories", CreatedExcludedTopLevelDirectories());
            SetPrivateField(CurrentBuildManager, "_forbiddenTopLevelDirectories", CreateForbiddenTopLevelDirectories());

            var preStartInitStageEnum = typeof(BuildManager).Assembly.GetTypes()
                .First(t => t.Name == "PreStartInitStage");
            var preStartInitStageProperty = typeof (BuildManager).GetProperty("PreStartInitStage",
                BindingFlags.Static | BindingFlags.NonPublic);
            preStartInitStageProperty.SetValue(null, Enum.Parse(preStartInitStageEnum, "AfterPreStartInit"));
        }

        public static object CreateNonRelativeTrailingSlash(string vpath)
        {
            var createNonRelativeTrailingSlash = VirtualPathType
                .GetMethod("CreateNonRelativeTrailingSlash", BindingFlags.Static | BindingFlags.Public);

            return createNonRelativeTrailingSlash.Invoke(null, new object[] { vpath });
        }

        public static object CreateForbiddenTopLevelDirectories()
        {
            return CreateStringSet(CodeDirectoryName, ResourcesDirectoryName, LocalResourcesDirectoryName, WebRefDirectoryName, ThemesDirectoryName);
        }

        public static object CreatedExcludedTopLevelDirectories()
        {
            return CreateStringSet(BinDirectoryName, CodeDirectoryName, ResourcesDirectoryName, LocalResourcesDirectoryName, WebRefDirectoryName, ThemesDirectoryName);
        }

        private static ICollection CreateStringSet(params string[] directories)
        {
            var stringSetType = typeof (HttpRuntime).Assembly.GetTypes()
                .First(t => t.Name == "CaseInsensitiveStringSet");

            var stringSet = Activator.CreateInstance(stringSetType) as ICollection;

            var addCollectionMethod = stringSetType.GetMethod("AddCollection");
            addCollectionMethod.Invoke(stringSet, new object[] {directories.ToList()});
            return stringSet;
        }

        public static object CreateVirtualPath(string vpath)
        {
            return VirtualPathType.GetMethod("Create", new[] { typeof(string) })
                .Invoke(null, new object[] { vpath });
        }


        public static object GetPrivatePropertyValue<T>(T instance, string propertyName)
        {
            var property = typeof(T).GetProperty(propertyName,
                BindingFlags.NonPublic | (instance == null ? BindingFlags.Static : BindingFlags.Instance));
            return property.GetValue(instance, null);
        }

        public static object GetPublicPropertyValue<T>(T instance, string propertyName)
        {
            var property = typeof(T).GetProperty(propertyName,
                BindingFlags.Public | (instance == null ? BindingFlags.Static : BindingFlags.Instance));
            return property.GetValue(instance, null);
        }

        public static string GetVirtualPathString(object virtualPath)
        {
            var property = VirtualPathType.GetProperty("VirtualPathString", BindingFlags.Public | BindingFlags.Instance);
            return property.GetValue(virtualPath) as string;
        }

        public static string SimpleCombine(object virtualPath)
        {
            var property = VirtualPathType.GetProperty("VirtualPathString", BindingFlags.Public | BindingFlags.Instance);
            return property.GetValue(virtualPath) as string;
        }

        public static HttpRuntime CurrentHttpRuntime
        {
            get
            {
                var runtimeField = typeof(HttpRuntime).GetField("_theRuntime", BindingFlags.NonPublic | BindingFlags.Static);
                Debug.Assert(runtimeField != null, "runtimeField != null");
                var runtime = (HttpRuntime)runtimeField.GetValue(null);
                return runtime;
            }
        }

        public static BuildManager CurrentBuildManager
        {
            get
            {
                if (_buildManager == null)
                {
                    var runtimeField = typeof (BuildManager).GetField("_theBuildManager",
                        BindingFlags.NonPublic | BindingFlags.Static);
                    Debug.Assert(runtimeField != null, "_theBuildManager != null");

                    _buildManager = (BuildManager) runtimeField.GetValue(null);
                }
                return _buildManager;
            }
        }

        public static void SetPrivateField<T>(T runtime, string fieldName, object value)
        {
            Console.WriteLine("set private field {0} = {1}", fieldName, value);
            var privateField = typeof(T).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            Debug.Assert(privateField != null, "privateField != null");
            privateField.SetValue(runtime, value);
        }

        public static void Initialize()
        {
            if (_initialized)
                return;

            _initialized = true;
            var appDomainAppVPath = CreateNonRelativeTrailingSlash("/");

            SetPrivateField(CurrentHttpRuntime, "_appDomainAppId", "SimulatorApp");
            SetPrivateField(CurrentHttpRuntime, "_appDomainAppPath", AppDomainAppPath);
            SetPrivateField(CurrentHttpRuntime, "_appDomainAppVPath", appDomainAppVPath);
            // SetPrivateField(HttpRuntime, "_appDomainId", "SimulatorDomain");
            SetPrivateField(CurrentHttpRuntime, "_isOnUNCShare", false);
            SetPrivateField(CurrentHttpRuntime, "_codegenDir", @"C:\Dev\temp\codegen");

            string appDomainAppVirtualPathString = GetPrivatePropertyValue<HttpRuntime>(null, "AppDomainAppVirtualPathString") as string;
            Debug.Assert(appDomainAppVirtualPathString != null);
            Debug.Assert(HttpRuntime.AppDomainAppVirtualPath != null);
            Debug.Assert(AppDomain.CurrentDomain != null);

            try
            {
                var hostingEnvironment = CreateHostingEnvironment(appDomainAppVPath);

                ValidateVirtualPathInternal(AppDomainAppPath + @"\Dummy.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                throw ex.InnerException;
            }
        }

        private static HostingEnvironment CreateHostingEnvironment(object appDomainAppVPath)
        {
            var hostingEnvironment = new HostingEnvironment();
            var providerField = typeof(HostingEnvironment).GetField("_virtualPathProvider",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Debug.Assert(providerField != null, "providerProperty != null");

            providerField.SetValue(hostingEnvironment, new VirtualPathProviderSimulator());

            SetPrivateField(hostingEnvironment, "_appId", "SimulatorApp");
            SetPrivateField(hostingEnvironment, "_appVirtualPath", appDomainAppVPath);
            string appPhysicalPath;
            SetPrivateField(hostingEnvironment, "_appPhysicalPath", appPhysicalPath = GetPrivatePropertyValue((HttpRuntime)null, "AppDomainAppPathInternal") as string);

            // IConfigMapPathFactory configMapPathFactory = new ConfigMapPathFactorySimulator();
            var configMapPath = new ConfigMapPathSimulator();
            SetPrivateField(hostingEnvironment, "_configMapPath", configMapPath);

            // detach unload event
            var onAppDomainUnloadField = typeof (HostingEnvironment).GetField("_onAppDomainUnload",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Debug.Assert(onAppDomainUnloadField != null, "onAppDomainUnloadField != null");
            var onAppDomainUnload = (EventHandler)onAppDomainUnloadField.GetValue(hostingEnvironment);

            Thread.GetDomain().DomainUnload -= onAppDomainUnload;


            return hostingEnvironment;
        }

        private static void ValidateVirtualPathInternal(string viewPath)
        {
            var codegenDirInternal = GetPrivatePropertyValue((HttpRuntime)null, "CodegenDirInternal");
            Debug.Assert(codegenDirInternal != null);

            var appDomainAppVirtualPathObject = GetPrivatePropertyValue<HttpRuntime>(null, "AppDomainAppVirtualPathObject");
            Debug.Assert(appDomainAppVirtualPathObject != null);
            var appDomainAppVirtualPathString = (string)GetPrivatePropertyValue<HttpRuntime>(null, "AppDomainAppVirtualPathString");
            Debug.Assert(appDomainAppVirtualPathString != null);

            int length = appDomainAppVirtualPathString.Length;

            var virtualPath = CreateVirtualPath(viewPath);
            Debug.Assert(virtualPath != null);
            var virtualPathString = GetVirtualPathString(virtualPath);
            Debug.Assert(virtualPathString != null);

            var method = typeof(BuildManager).GetMethod("ValidateVirtualPathInternal",
                BindingFlags.NonPublic | BindingFlags.Instance);

            VirtualPathType.GetMethod("FailIfNotWithinAppRoot", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(virtualPath, null);


            method.Invoke(CurrentBuildManager, new[] { virtualPath, false, false });

        }
    }

    internal class VirtualPathProviderSimulator : VirtualPathProvider
    {
        public override bool FileExists(string virtualPath)
        {
            return true;
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            return base.GetFile(virtualPath);
        }

        public override string GetFileHash(string virtualPath, IEnumerable virtualPathDependencies)
        {
            return base.GetFileHash(virtualPath, virtualPathDependencies);
        }

        public override bool DirectoryExists(string virtualDir)
        {
            return base.DirectoryExists(virtualDir);
        }

        public override VirtualDirectory GetDirectory(string virtualDir)
        {
            return base.GetDirectory(virtualDir);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }

        public override object InitializeLifetimeService()
        {
            return base.InitializeLifetimeService();
        }

        public override string CombineVirtualPaths(string basePath, string relativePath)
        {
            return base.CombineVirtualPaths(basePath, relativePath);
        }

        public override string GetCacheKey(string virtualPath)
        {
            return base.GetCacheKey(virtualPath);
        }
    }
}