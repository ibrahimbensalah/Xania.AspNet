using System;
using System.Collections.Generic;
using System.IO;

namespace Xania.AspNet.Core
{
    public interface IContentProvider
    {
        Stream Open(string relativePath);
        bool Exists(string relativePath);
        string GetPhysicalPath(string relativePath);
        string GetRelativePath(string physicalPath);
        IEnumerable<string> GetFiles(string searchPattern);
        DateTime GetModifiedDateTime(string relativePath);
    }
}