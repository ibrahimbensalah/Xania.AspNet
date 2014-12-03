using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using NUnit.Framework;

namespace Xania.AspNet.Simulator.Tests
{
    public class LinqActionValidationTests
    {
        [Test]
        public void InvalidModelTest()
        {
            // arrange
            var controllerAction = new AccountController().Action(c => c.ChangePassword(new ChangePasswordModel()));

            // act
            var result = controllerAction.Execute();
            
            // assert
            Assert.IsFalse(result.ModelState.IsValid);
        }

        [Test]
        public void ValidModelTest()
        {
            // arrange
            var controllerAction = new AccountController().Action(c => c.ChangePassword(new ChangePasswordModel
            {
                Email = "ibrahim@simulator.com",
                NewPassword = "password",
                ConfirmPassword = "password"
            }));

            // act
            var result = controllerAction.Execute();

            // assert
            Assert.IsTrue(result.ModelState.IsValid);
        }

        private class AccountController: Controller
        {
            public void ChangePassword(ChangePasswordModel model)
            {
                if (model == null) throw new ArgumentNullException("model");
            }
        }

        private class ChangePasswordModel
        {
            [Required]
            public String Email { get; set; }

            [Required]
            public String NewPassword { get; set; }

            [Required, System.ComponentModel.DataAnnotations.Compare("NewPassword")]
            public String ConfirmPassword { get; set; }
        }
    }
}
