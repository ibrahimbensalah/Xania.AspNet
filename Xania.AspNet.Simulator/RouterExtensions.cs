using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xania.AspNet.Core;

namespace Xania.AspNet.Simulator
{
    public static class RouterExtensions
    {
        public static HttpControllerAction Action(this ControllerContainer controllerContainer, string url)
        {
            var mvcApplication = new MvcApplication(controllerContainer, new EmptyContentProvider());
            return new HttpControllerAction(mvcApplication) { UriPath = url };
        }

        public static HttpControllerAction ParseAction(this ControllerContainer controllerContainer, string rawHttpRequest)
        {
            var lines = rawHttpRequest.Split('\n');
            var first = lines.First();

            var parts = first.Split(' ');
            return new HttpControllerAction(new MvcApplication(controllerContainer, new EmptyContentProvider()))
            {
                HttpMethod = parts[0],
                UriPath = parts[1]
            };
        }
    }

    public class EmptyContentProvider : IContentProvider
    {
        public Stream Open(string relativePath)
        {
            throw new InvalidOperationException();
        }

        public bool Exists(string relativePath)
        {
            return false;
        }

        public string GetPhysicalPath(string relativePath)
        {
            throw new InvalidOperationException();
        }

        public string GetRelativePath(string physicalPath)
        {
            throw new InvalidOperationException();
        }

        public IEnumerable<string> GetFiles(string searchPattern)
        {
            throw new NotImplementedException();
        }
    }
}
