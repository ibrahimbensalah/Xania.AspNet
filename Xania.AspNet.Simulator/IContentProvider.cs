using System.IO;

namespace Xania.AspNet.Simulator
{
    public interface IContentProvider
    {
        Stream Open(string relativePath);
        bool Exists(string relativePath);
    }
}