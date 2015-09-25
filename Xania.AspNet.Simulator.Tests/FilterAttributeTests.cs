using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Xania.AspNet.Simulator.Tests
{
    public class FilterAttributeTests
    {
        [Test]
        public void AuthorizationFilterWithAnonymousUserTest()
        {
            // arrange
            var context = new PersonController()
                .Action(c => c.Index(new PersonModel()))
                .GetExecutionContext()
                .GetAuthorizationContext();
            // act
            new AuthorizeAttribute().OnAuthorization(context);
            // assert
            context.Result.Should().BeOfType<HttpUnauthorizedResult>();
        }

        [Test]
        public void AuthorizationFilterWithAuthenticatedUserTest()
        {
            // arrange
            var context = new PersonController()
                .Action(c => c.Index(new PersonModel()))
                .Authenticate("userName1", new string[0])
                .GetExecutionContext()
                .GetAuthorizationContext();
            // act
            new AuthorizeAttribute().OnAuthorization(context);
            // assert
            context.Result.Should().BeNull();
        }

        [Test]
        public void ActionExecutingContextTest()
        {
            // arrange
            var context = GetExecutionContext()
                .GetActionExecutingContext();
            // asserts
            context.ActionParameters.Count().Should().Be(1);
            context.ActionParameters["model"].Should().BeOfType<PersonModel>();
            context.IsChildAction.Should().BeTrue();
            context.RouteData.GetRequiredString("controller").Should().BeEquivalentTo("person");
        }

        [Test]
        public void ActionExecutedContextTest()
        {
            // arrange
            var context = GetExecutionContext()
                .GetActionExecutedContext();
            // asserts
            context.IsChildAction.Should().BeTrue();
            context.RouteData.GetRequiredString("controller").Should().BeEquivalentTo("person");
        }

        [Test]
        public void ResultExecutingContextTest()
        {
            // arrange
            var context = GetExecutionContext()
                .GetResultExecutingContext(Substitute.For<ActionResult>());
            // asserts
            context.RouteData.GetRequiredString("controller").Should().BeEquivalentTo("person");
        }

        [Test]
        public void ResultExecutedContextTest()
        {
            // arrange
            var context = GetExecutionContext()
                .GetResultExecutedContext(Substitute.For<ActionResult>());
            // asserts
            context.Should().NotBeNull();
            context.RouteData.GetRequiredString("controller").Should().BeEquivalentTo("person");
        }

        private ActionExecutionContext GetExecutionContext()
        {
            return new PersonController()
                .Action(c => c.Index(new PersonModel()))
                .IsChildAction()
                .GetExecutionContext();
        }

        class PersonController: Controller
        {
            public void Index(PersonModel model)
            {
            }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        class PersonModel
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
    }
}