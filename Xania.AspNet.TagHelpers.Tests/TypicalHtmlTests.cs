using System.IO;
using FluentAssertions;
using NUnit.Framework;

namespace Xania.AspNet.TagHelpers.Tests
{
    public class TypicalHtmlTests
    {
        [TestCase("typical.simple.html")]
        public void NoChangesTest(string resourceName)
        {
            var input = GetResourceContent(resourceName);

            var stringWriter = new StringWriter();
            var processor = new HtmlProcessor(stringWriter, new TagHelperContainer());

            processor.Write(input);
            var output = stringWriter.GetStringBuilder().ToString();

            output.Should().Be(input);
        }

        private static string GetResourceContent(string resourceName)
        {
            var thisType = typeof (TypicalHtmlTests);
            using (var stream = thisType.Assembly.GetManifestResourceStream(thisType, resourceName))
            {
                if (stream != null)
                    return new StreamReader(stream).ReadToEnd();
            }
            return null;
        }
    }
}
