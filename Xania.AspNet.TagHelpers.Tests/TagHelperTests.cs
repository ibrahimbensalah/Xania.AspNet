using System;
using System.IO;
using NUnit.Framework;
using Xania.AspNet.TagHelpers.Tests.Annotations;

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

        [UsedImplicitly]
        public class TagC: ITagHelper
        {
            public IRenderContext RenderContext { get; set; }

            public void WriteContent(TextWriter writer, char ch)
            {
                writer.Write(Char.ToUpper(ch));
            }

            public void RenderAfterContent(TextWriter writer)
            {
                writer.Write("</span></h1></div>");
            }

            public void RenderBeforeContent(TextWriter writer)
            {
                writer.Write("<div><h1>");
                writer.Write(RenderContext.GetValue("P1"));
                writer.Write("<span>");
            }
        }
    }
}
