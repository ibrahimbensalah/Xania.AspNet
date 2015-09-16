using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using Xania.AspNet.Core;

namespace Xania.AspNet.Razor
{
    public abstract class BundlesBase
    {
        private readonly HttpContextBase _context;
        private readonly IMvcApplication _mvcApplication;

        protected BundlesBase(HttpContextBase context, IMvcApplication mvcApplication)
        {
            _context = context;
            _mvcApplication = mvcApplication;
        }

        public IHtmlString Render(params string[] paths)
        {
            var stringBuilder = new StringBuilder();
            foreach (var path in paths)
            {
                foreach (var content in GetBundleContents(path))
                    stringBuilder.Append(GetHtml(content));
            }

            return MvcHtmlString.Create(stringBuilder.ToString());
        }

        protected abstract string GetHtml(string path);

        private IEnumerable<string> GetBundleContents(string path)
        {
            var bundleContext = GetBundleContext(path);

            return from bundle in _mvcApplication.Bundles
                where bundle.Path == path
                from BundleFile f in bundle.EnumerateFiles(bundleContext)
                let virtualFile = f.VirtualFile as VirtualFileWrapper
                select _mvcApplication.ToAbsoluteUrl(virtualFile.VirtualPathString);
        }

        private static readonly object SyncObject = new object();

        private BundleContext GetBundleContext(string path)
        {
            lock (SyncObject)
            {
                var originalPathProvider = BundleTable.VirtualPathProvider;
                var originalEnableOptimizations = BundleTable.EnableOptimizations;

                try
                {
                    BundleTable.VirtualPathProvider = new VirtualPathProviderSimulator(_mvcApplication);
                    BundleTable.EnableOptimizations = false;

                    return new BundleContext(_context, _mvcApplication.Bundles, path);
                }
                finally
                {
                    BundleTable.VirtualPathProvider = originalPathProvider;
                    BundleTable.EnableOptimizations = originalEnableOptimizations;
                }
            }
        }

        public IHtmlString Url(string virtualPath)
        {
            return MvcHtmlString.Create("http://www.google.nl");
        }
    }

    internal class VirtualPathProviderSimulator : VirtualPathProvider
    {
        private readonly IMvcApplication _mvcApplication;

        public VirtualPathProviderSimulator(IMvcApplication mvcApplication)
        {
            _mvcApplication = mvcApplication;
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            return new VirtualFileWrapper(_mvcApplication.GetVirtualContent(virtualPath));
        }

        public override VirtualDirectory GetDirectory(string virtualDir)
        {
            return new VirtualDirectoryWrapper(_mvcApplication.GetVirtualDirectory(virtualDir));
        }
    }

    internal class VirtualDirectoryWrapper : VirtualDirectory
    {
        private readonly IVirtualDirectory _virtualDirectory;

        public VirtualDirectoryWrapper(IVirtualDirectory virtualDirectory)
            : base(virtualDirectory.VirtualPath)
        {
            _virtualDirectory = virtualDirectory;
        }

        public override IEnumerable Directories
        {
            get { throw new NotImplementedException(); }
        }

        public override IEnumerable Files
        {
            get { return _virtualDirectory.GetFiles().Select(vf => new VirtualFileWrapper(vf)); }
        }

        public override IEnumerable Children
        {
            get { throw new NotImplementedException(); }
        }
    }

    internal class VirtualFileWrapper : VirtualFile
    {
        private readonly IVirtualContent _virtualContent;

        public VirtualFileWrapper(IVirtualContent virtualContent) 
            : base(virtualContent.VirtualPath)
        {
            _virtualContent = virtualContent;
        }

        public override string Name
        {
            get { return _virtualContent.VirtualPath; }
        }

        public string VirtualPathString { get { return _virtualContent.VirtualPath; } }

        public override Stream Open()
        {
            return _virtualContent.Open();
        }
    }
}
