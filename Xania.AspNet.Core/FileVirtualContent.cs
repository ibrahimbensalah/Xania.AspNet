using System;
using System.IO;

namespace Xania.AspNet.Core
{
    public class FileVirtualContent : IVirtualContent
    {
        private readonly IContentProvider _contentProvider;
        private readonly string _virtualPath;
        private readonly string _filePath;

        public FileVirtualContent(IContentProvider contentProvider, string virtualPath)
        {
            _contentProvider = contentProvider;
            _virtualPath = virtualPath;
            _filePath = ToFilePath(virtualPath);
        }

        public DateTime ModifiedDateTime
        {
            get { return File.GetLastAccessTime(_filePath); }
        }

        public Stream Open()
        {
            return _contentProvider.Open(_filePath);
        }
        private string ToFilePath(string virtualPath)
        {
            return virtualPath.Substring(2).Replace("/", "\\");
        }

        public string VirtualPath
        {
            get { return _virtualPath; }
        }

        public bool Exists
        {
            get { return _contentProvider.Exists(_filePath); }
        }
    }
}