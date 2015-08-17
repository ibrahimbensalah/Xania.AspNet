using System;
using System.IO;

namespace Xania.AspNet.Core
{
    public interface IVirtualContent
    {
        DateTime ModifiedDateTime { get; }

        Stream Open();

        string VirtualPath { get; }

        bool Exists { get; }
    }
}