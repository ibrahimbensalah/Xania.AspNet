using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xania.AspNet.Core
{
    public interface IWebViewPageFactory
    {
        IWebViewPage Create(string virtualPath, TextReader reader, DateTime modifiedDateTime);
    }
}
