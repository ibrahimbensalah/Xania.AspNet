using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
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
                let virtualFile = f.VirtualFile
                select _mvcApplication.ToAbsoluteUrl(virtualFile.VirtualPath);
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
            return VirtualFileProxy.Create(_mvcApplication.GetVirtualContent(virtualPath));
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

        public override string Name
        {
            get { return _virtualDirectory.VirtualPath.Split('/').Last(); }
        }

        public override IEnumerable Directories
        {
            get { throw new NotImplementedException(); }
        }

        public override IEnumerable Files
        {
            get { return _virtualDirectory.GetFiles().Select(VirtualFileProxy.Create); }
        }

        public override IEnumerable Children
        {
            get { throw new NotImplementedException(); }
        }
    }

    internal class VirtualFileProxy : RealProxy
    {
        private readonly IVirtualContent _virtualContent;

        public VirtualFileProxy(IVirtualContent virtualContent)
            : base(typeof(VirtualFile))
        {
            _virtualContent = virtualContent;
        }

        public override IMessage Invoke(IMessage msg)
        {
            var methodCall = msg as IMethodCallMessage;
            if (methodCall == null)
                return null;

            try
            {
                var methodName = methodCall.MethodName;
                object result = null;
                switch (methodName)
                {
                    case "get_VirtualPath":
                        result = _virtualContent.VirtualPath;
                        break;
                    case "get_Name":
                        result = _virtualContent.VirtualPath.Split('/', '\\').Last();
                        break;
                    default:
                        throw new NotImplementedException(methodName);
                }

                return new ReturnMessage(result, null, 0, methodCall.LogicalCallContext, methodCall);
            }
            catch (TargetInvocationException invocationException)
            {
                var exception = invocationException.InnerException;
                return new ReturnMessage(exception, methodCall);
            }
        }

        public static VirtualFile Create(IVirtualContent vf)
        {
            var proxy = new VirtualFileProxy(vf);
            return (VirtualFile) proxy.GetTransparentProxy();
        }
    }
}
