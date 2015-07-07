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
        public static IWebViewPage CreatePage(this IMvcApplication mvcApplication, IVirtualContent virtualContent, bool includeStartPage)
        {
            using (var reader = mvcApplication.OpenText(virtualContent.VirtualPath, includeStartPage))
            {
                return new WebViewPageFactory(mvcApplication.Assemblies).Create(virtualContent.VirtualPath, reader, virtualContent.ModifiedDateTime);
            }
        }

        public static IMvcApplication EnableRazor(this IMvcApplication mvcApplication)
        {
            mvcApplication.ViewEngines.Add(new RazorViewEngineSimulator(mvcApplication));

            BundleTable.MapPathMethod = mvcApplication.MapPath;
            DisplayModeProvider.Instance.Modes.Clear();
            DisplayModeProvider.Instance.Modes.Add(new SimpleDisplayMode());

            return mvcApplication;
        }

        public static IMvcApplication WithBundles(this IMvcApplication mvcApplication, Action<BundleCollection> registerBundles)
        {
            registerBundles(mvcApplication.Bundles);

            return mvcApplication;
        }
    }
}
