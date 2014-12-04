using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using NUnit.Framework;

namespace Xania.AspNet.Simulator.Tests
{
    public class LinqActionValidationTests
    {
        [TestCase(null, null, false, false)]
        [TestCase("pass1", "pass1", true, true)]
        [TestCase("pass1", "pass2", true, false)]
        public void ModelStateTest(string newPassword, string confirmPassword, bool newPasswordValid, bool confirmPasswordValid)
        {
            // arrange
            var model = new ChangePasswordModel { NewPassword = newPassword, ConfirmPassword = confirmPassword };
            var controllerAction = new AccountController().Action(c => c.ChangePassword(model, null));

            // act
            var result = controllerAction.Execute();

            // assert
            Assert.AreEqual(newPasswordValid, result.ModelState.IsValidField("model.NewPassword"));
            Assert.AreEqual(confirmPasswordValid, result.ModelState.IsValidField("model.ConfirmPassword"));
        }

        private class AccountController : Controller
        {
            public ActionResult ChangePassword(ChangePasswordModel model, ChangePasswordModel model2)
            {
                if (model == null) throw new ArgumentNullException("model");

                return null;
            }
        }

        private class ChangePasswordModel
        {
            [Required]
            public String NewPassword { get; set; }

            [Required, System.ComponentModel.DataAnnotations.Compare("NewPassword")]
            public String ConfirmPassword { get; set; }
        }
    }
}
