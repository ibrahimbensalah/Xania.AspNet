using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Xania.AspNet.Core
{
    public interface IVirtualContent
    {
        DateTime ModifiedDateTime { get; }

        Stream Open();

        string VirtualPath { get; }

        bool Exists { get; }
    }


    public interface IVirtualDirectory
    {
        IEnumerable<IVirtualContent> GetFiles();
        string VirtualPath { get; }
    }

    public class PhysicalVirtualDirectory : IVirtualDirectory
    {
        private readonly IContentProvider _contentProvider;
        private readonly string _virtualPath;
        private readonly string _relativePath;

        public PhysicalVirtualDirectory(IContentProvider contentProvider, string virtualPath)
        {
            _contentProvider = contentProvider;
            _virtualPath = virtualPath;
            _relativePath = ToRelativePath(virtualPath);
        }

        public IEnumerable<IVirtualContent> GetFiles()
        {
            return from path in _contentProvider.GetFiles(_relativePath)
                let fi = new FileInfo(path)
                select new FileVirtualContent(_contentProvider, _virtualPath + "/" + fi.Name);
        }

        public string VirtualPath { get { return _virtualPath; } }

        private string ToRelativePath(string virtualPath)
        {
            return virtualPath.Substring(2).Replace("/", "\\");
        }
    }
}