namespace Xania.AspNet.TagHelpers
{
    public static class TagHelperExtensions
    {
        public static void Register<TTagHelper>(this ITagHelperContainer tagHelperContainer, string tagName)
        {
            tagHelperContainer.Register(tagName, typeof(TTagHelper));
        }
    }
}