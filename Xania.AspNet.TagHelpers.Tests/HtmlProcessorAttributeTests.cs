using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NSubstitute;
using NUnit.Framework;

namespace Xania.AspNet.TagHelpers.Tests
{
    public class HtmlProcessorAttributeTests
    {

        [Test]
        public void WhenExecutingTagHelperThenViewContextIsInjected()
        {
            // arrange
            //var provider = Substitute.For<ITagHelperProvider>();
            //var attr = new TagHelperFilterAttribute();
            //var context = new Mock<ResultExecutingContext>();
            //// act
            //attr.OnResultExecuting(context.Object);
            //// assert
            //// provider.Received().
        }


    }
}
