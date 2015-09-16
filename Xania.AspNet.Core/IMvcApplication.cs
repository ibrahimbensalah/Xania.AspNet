using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Xania.AspNet.Core
{
    public interface IMvcApplication
    {
        RouteCollection Routes { get; }

        IContentProvider ContentProvider { get; }

        IControllerFactory ControllerFactory { get; }

        ViewEngineCollection ViewEngines { get; }

        FilterProviderCollection FilterProviders { get; }

        IWebViewPageFactory WebViewPageFactory { get; }

        BundleCollection Bundles { get; }

        IHtmlString Action(ViewContext viewContext, string actionName, object routeValues);

        IEnumerable<string> Assemblies { get; }

        IValueProvider ValueProvider { get; set; }

        string MapUrl(string filePath);

        bool FileExists(string virtualPath);

        string MapPath(string virtualPath);

        string ToAbsoluteUrl(string path);

        IVirtualContent GetVirtualContent(string virtualPath);

        IValueProvider GetValueProvider(ControllerContext controllerContext);

        IEnumerable<ModelValidationResult> ValidateModel(Type modelType, object modelValue,
            ControllerContext controllerContext);

        IVirtualDirectory GetVirtualDirectory(string virtualPath);
    }
}