using System;
using System.Collections.Generic;

namespace Xania.AspNet.TagHelpers
{
    public interface ITagHelperProvider
    {
        ITagHelper GetTagHelper(string tagName, IDictionary<string, string> attributes);
    }

    public interface ITagHelperContainer: ITagHelperProvider
    {
        void Register(string tagName, Type type);
    }
}