using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.WebPages;
using Xania.AspNet.Core;

namespace Xania.AspNet.Razor
{
    public static class MvcApplicationExtensions
    {
        public static IWebViewPage Create(this IMvcApplication mvcApplication, string virtualPath)
        {
            using (var reader = mvcApplication.OpenText(virtualPath, false))
            {
                return new WebViewPageFactory().Create(virtualPath, reader);
            }
        }

        public static IWebViewPage Create(this IMvcApplication mvcApplication, ViewContext viewContext, string virtualPath, TextReader reader)
        {
            var webPage = new WebViewPageFactory().Create(virtualPath, reader);
            webPage.Initialize(viewContext, virtualPath, mvcApplication);

            return webPage;
        }

        public static IMvcApplication EnableRazor(this IMvcApplication mvcApplication)
        {
            DisplayModeProvider.Instance.Modes.Clear();
            DisplayModeProvider.Instance.Modes.Add(new DisplayModeSimulator());

            mvcApplication.ViewEngines.Add(new RazorViewEngineSimulator(mvcApplication));

            BundleTable.MapPathMethod = mvcApplication.GetPhysicalPath;

            return mvcApplication;
        }

        public static void RegisterBundles(this IMvcApplication mvcApplication, Action<BundleCollection> cfg)
        {
            var startTime = DateTime.Now;
            var bundles = new BundleCollection();
            cfg(bundles);

            var baseDirectory = mvcApplication.GetPhysicalPath("~/");

            foreach (var bundle in bundles)
            {
                var bundle1 = bundle;
                Func<HttpContextBase, IEnumerable<string>> factory = context =>
                {
                    var bundleContext = new BundleContext(context, bundles, bundle1.Path);
                    return from f in bundle1.EnumerateFiles(bundleContext)
                        select f.FullName.Substring(baseDirectory.Length).Replace("\\", "/");
                };


                mvcApplication.Bundles.Add(new Core.Bundle(bundle1.Path, factory));
            }
            var endTime = DateTime.Now;
            Console.WriteLine("RegisterBundles " + (endTime - startTime).TotalMilliseconds);
        }
    }
}
