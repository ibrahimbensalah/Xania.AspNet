using System.IO;

namespace Xania.AspNet.Core
{
    public interface IContentProvider
    {
        Stream Open(string relativePath);
        bool Exists(string relativePath);
        string GetPhysicalPath(string relativePath);
    }
}