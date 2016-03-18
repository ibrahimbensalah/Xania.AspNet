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
            get { return _contentProvider.GetModifiedDateTime(_filePath); }
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
            get { return _contentProvider.FileExists(_filePath); }
        }
    }

    public class StreamVirtualContent : IVirtualContent
    {
        private readonly Stream _stream;

        public StreamVirtualContent(string virtualPath, Stream stream)
        {
            _stream = stream;
            VirtualPath = virtualPath;
            ModifiedDateTime = DateTime.Now;
            Exists = true;
        }

        public DateTime ModifiedDateTime { get; private set; }
        public Stream Open()
        {
            return _stream;
        }

        public string VirtualPath { get; private set; }
        public bool Exists { get; private set; }
    }

    public class StringVirtualContent : IVirtualContent
    {
        private readonly string _content;

        public DateTime ModifiedDateTime { get; private set; }

        public StringVirtualContent(string virtualPath, string content)
        {
            _content = content;
            ModifiedDateTime = DateTime.Now;
            VirtualPath = virtualPath;
        }

        public Stream Open()
        {
            return GenerateStreamFromString(_content);
        }

        public string VirtualPath { get; private set; }
        public bool Exists { get { return true; } }

        public Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}