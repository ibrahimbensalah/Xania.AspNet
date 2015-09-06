using System;
using System.IO;

namespace Xania.AspNet.Core
{
    public static class ContentProviderExtensions
    {
        public static TextReader Open(this IContentProvider contentProvider, string relativePath, bool includeStartPage)
        {
            var contentStream = contentProvider.Open(relativePath);
            const string startPagePath = @"Views\_ViewStart.cshtml";

            return includeStartPage && !String.Equals(relativePath, startPagePath) &&
                   contentProvider.FileExists(startPagePath)
                ? (TextReader)new ConcatenatedStream(contentProvider.Open(@"Views\_ViewStart.cshtml"), contentStream)
                : new StreamReader(contentStream);
        }
    }
}
