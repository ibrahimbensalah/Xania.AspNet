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

        private static readonly object SyncObject = new object();

        public static IMvcApplication WithBundles(this IMvcApplication mvcApplication, Action<BundleCollection> registerBundles)
        {
            lock (SyncObject)
            {
                var originalMapPathMethod = BundleTable.MapPathMethod;
                try
                {
                    BundleTable.MapPathMethod = mvcApplication.MapPath;
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
}
