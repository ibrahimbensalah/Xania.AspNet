using System;
using System.IO;
using System.Web.Optimization;
using System.Web.WebPages;
using Xania.AspNet.Core;

namespace Xania.AspNet.Razor
{
    public static class MvcApplicationExtensions
    {
        public static IWebViewPage CreatePage(this IMvcApplication mvcApplication, IVirtualContent virtualContent,
            bool includeStartPage)
        {
            var contentStream = virtualContent.Open();
            var startPagePath = mvcApplication.GetVirtualContent(@"~/Views/_ViewStart.cshtml");

            var reader = includeStartPage && startPagePath.Exists &&
                         !String.Equals(virtualContent.VirtualPath, startPagePath.VirtualPath)
                ? (TextReader)new ConcatenatedStream(startPagePath.Open(), contentStream)
                : new StreamReader(contentStream);

            using (reader)
            {
                return new WebViewPageFactory(mvcApplication.Assemblies).Create(virtualContent.VirtualPath, reader,
                    virtualContent.ModifiedDateTime);
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
