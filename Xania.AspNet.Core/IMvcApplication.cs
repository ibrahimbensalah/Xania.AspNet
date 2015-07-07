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

        ViewEngineCollection ViewEngines { get; }

        IContentProvider ContentProvider { get; }

        IControllerFactory ControllerFactory { get; }

        IHtmlString Action(ViewContext viewContext, string actionName, object routeValues);

        TextReader OpenText(string virtualPath, bool includeStartPage);

        BundleCollection Bundles { get; }

        IEnumerable<string> Assemblies { get; }

        string MapUrl(FileInfo file);

        bool Exists(string virtualPath);

        string MapPath(string virtualPath);

        string ToAbsoluteUrl(string path);

        IVirtualContent GetVirtualContent(string virtualPath);
    }
}