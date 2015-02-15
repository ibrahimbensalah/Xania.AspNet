using System;
using System.IO;
using NUnit.Framework;

namespace Xania.AspNet.TagHelpers.Tests
{
    public class TagHelperTests
    {
        [TestCase("< a >< / a>", "<a></a>")]
        [TestCase("<a attrs />", "<a attrs=\"\" />")]
        [TestCase("<a       attrs />", "<a attrs=\"\" />")]
        [TestCase("<a    attrs=asdfas a=b />", "<a attrs=\"asdfas\" a=\"b\" />")]
        [TestCase("<a a=\"b\" />", "<a a=\"b\" />")]
        [TestCase("<a><b >x< /b></a>", "<a><b>x</b></a>")]
        [TestCase("<a a=\"b\" />", "<a a=\"b\" />")]
        [TestCase("<c P1=\"a &euro;\" />", "<div><h1>a &euro;<span></span></h1></div>")]
        [TestCase("<c P1=\"a &euro;\" >x</c>", "<div><h1>a &euro;<span>X</span></h1></div>")]
        public void TransformTest(String input, string expected)
        {
            // arrange
            var writer = new StringWriter();
            var mng = new HtmlProcessor(writer).Register<TagC>("c");
            // act
            mng.Write(input).Flush();
            // assert
            Assert.AreEqual(expected, writer.GetStringBuilder().ToString());
        }

        [TestCase("<a controller=\"Home\" action=\"Index\">Home</a>", "<a href=\"/home/index\">Home</a>")]
        public void ActionLinkTest(String input, string expected)
        {
            // arrange
            var writer = new StringWriter();
            var mng = new HtmlProcessor(writer).Register<AnchorHelper>("a");
            // act
            mng.Write(input).Flush();
            // assert
            Assert.AreEqual(expected, writer.GetStringBuilder().ToString());
        }

    }

    public class AnchorHelper: ITagHelper
    {
        public IRenderContext RenderContext { get; set; }

        public void WriteContent(TextWriter writer, char ch)
        {
            writer.Write(ch);
        }

        public void RenderAfterContent(TextWriter writer)
        {
            writer.Write("</a>");
        }

        public void RenderBeforeContent(TextWriter writer)
        {
            var action = RenderContext.GetValue("action");
            var controller = RenderContext.GetValue("controller");

            writer.Write("<a href=\"");
            writer.Write(String.Format("/{0}/{1}", controller, action).ToLowerInvariant());
            writer.Write("\">");
        }
    }
}
