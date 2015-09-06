using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.WebPages;
using System.Xml;
using Xania.AspNet.Core;

namespace Xania.AspNet.Razor
{
    public static class MvcApplicationExtensions
    {
        public static IWebViewPage CreatePage(this IMvcApplication mvcApplication, IVirtualContent virtualContent,
            bool includeStartPage)
        {
            const string startPagePath = @"~/Views/_ViewStart.cshtml";

            var contentStream = virtualContent.Open();
            var startPage = mvcApplication.GetVirtualContent(startPagePath);

            var reader = includeStartPage && startPage.Exists &&
                         !startPagePath.Equals(virtualContent.VirtualPath, StringComparison.OrdinalIgnoreCase)
                ? (TextReader)new ConcatenatedStream(startPage.Open(), contentStream)
                : new StreamReader(contentStream);

            using (reader)
            {
                return mvcApplication.WebViewPageFactory.Create(virtualContent.VirtualPath, reader,
                    virtualContent.ModifiedDateTime);
            }
        }

        private static IEnumerable<string> GetNamespaces(IMvcApplication mvcApplication)
        {
            if (mvcApplication.Exists("~/Views/Web.config"))
            {
                var virtualContent = mvcApplication.GetVirtualContent("~/Views/Web.config");
                var doc = new XmlDocument();
                using (var s = XmlReader.Create(virtualContent.Open()))
                {
                    doc.Load(s);

                    if (doc.DocumentElement != null)
                    {
                        var nodes = doc.DocumentElement
                            .SelectNodes("/configuration/system.web.webPages.razor/pages/namespaces/add");
                        if (nodes != null)
                            foreach (XmlNode n in nodes)
                            {
                                if (n.Attributes != null)
                                {
                                    var ns = n.Attributes["namespace"].Value;
                                    if (!ns.Equals("System.Web.Mvc.Html"))
                                        yield return ns;
                                    else
                                        yield return "Xania.AspNet.Razor.Html";
                                }
                            }
                    }
                }
            }
            else
            {
                var defaultList = new[]
                {
                    "System",
                    "System.Collections.Generic",
                    "System.Linq",
                    "System.Web.Mvc",
                    "System.Web.Mvc.Ajax",
                    "Xania.AspNet.Razor.Html",
                    "System.Web.Routing"
                };

                foreach (var ns in defaultList)
                    yield return ns;
            }

            var auth = mvcApplication.Assemblies.Any(a => a.EndsWith("Microsoft.Web.WebPages.OAuth.dll"));
            if (auth)
            {
                yield return "DotNetOpenAuth.AspNet";
                yield return "Microsoft.Web.WebPages.OAuth";
            }
        }

        public static IMvcApplication EnableRazor(this IMvcApplication mvcApplication)
        {
            mvcApplication.ViewEngines.Add(new RazorViewEngineSimulator(mvcApplication));
            mvcApplication.WebViewPageFactory = new WebViewPageFactory(mvcApplication.Assemblies, GetNamespaces(mvcApplication));
            
            return mvcApplication;
        }

        public static IMvcApplication WithBundles(this IMvcApplication mvcApplication, Action<BundleCollection> registerBundles)
        {
            var originalMapPathMethod = BundleTable.MapPathMethod;
            
            try
            {
                var bundleThread = Thread.CurrentThread;

                BundleTable.MapPathMethod = virtualPath =>
                {
                    if (bundleThread == Thread.CurrentThread)
                        return mvcApplication.MapPath(virtualPath);
                    else
                        return originalMapPathMethod(virtualPath);
                };

                registerBundles(mvcApplication.Bundles);
            }
            finally
            {
                BundleTable.MapPathMethod = originalMapPathMethod;
            }

            return mvcApplication;
        }
    }
}
