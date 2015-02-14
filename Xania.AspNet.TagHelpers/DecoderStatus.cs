using System;

namespace Xania.AspNet.TagHelpers
{
    [Flags]
    public enum DecoderStatus: ushort
    {
        TagName = 1,
        AttributeName = 2,
        AttributeValue = 4,
        Separator = 8
    }
}