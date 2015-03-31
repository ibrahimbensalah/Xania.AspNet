using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using NUnit.Framework;

namespace Xania.AspNet.Simulator.Tests.LinqActions
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
            var controllerAction = new AccountController().Action(c => c.ChangePassword(model));

            // act
            var result = controllerAction.Execute();

            // assert
            Assert.AreEqual(newPasswordValid, result.ModelState.IsValidField("model.NewPassword"));
            Assert.AreEqual(confirmPasswordValid, result.ModelState.IsValidField("model.ConfirmPassword"));
        }

        [Test]
        public void PrivitiveParameterTest()
        {
            // arrange
            var controllerAction = new AccountController().Action(e => e.DeleteUser(1));
            // act
            var result = controllerAction.Execute();
            //assert
            Assert.IsAssignableFrom<ContentResult>(result.ActionResult);
            Assert.AreEqual("Deleting User 1", (result.ActionResult as ContentResult).Content);
        }

        [Test]
        public void ExpressionParameterTest()
        {
            // arrange
            var controllerAction = new AccountController()
                .Action(e => e.DeleteUser(0))
                .Data(new {userId = "1"});
            // act
            var result = controllerAction.Execute();
            //assert
            Assert.IsAssignableFrom<ContentResult>(result.ActionResult);
            Assert.AreEqual("Deleting User 1", (result.ActionResult as ContentResult).Content);
        }

        private class AccountController : Controller
        {
            public ActionResult ChangePassword(ChangePasswordModel model)
            {
                if (model == null) throw new ArgumentNullException("model");

                return null;
            }

            public string DeleteUser(int? userId)
            {
                return String.Format("Deleting User {0}", userId);
            }
        }

        private class ChangePasswordModel
        {
            [Required]
            public String NewPassword { get; set; }

            [Required, Compare("NewPassword")]
            public String ConfirmPassword { get; set; }
        }
    }

    public class CompareAttribute : ValidationAttribute
    {

        public CompareAttribute(string otherProperty)
            : base("Must match")
        {
            if (otherProperty == null)
            {
                throw new ArgumentNullException("otherProperty");
            }
            OtherProperty = otherProperty;
        }

        public string OtherProperty { get; private set; }

        public string OtherPropertyDisplayName { get; internal set; }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, OtherPropertyDisplayName ?? OtherProperty);
        }

        public override bool RequiresValidationContext
        {
            get
            {
                return true;
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo otherPropertyInfo = validationContext.ObjectType.GetProperty(OtherProperty);
            if (otherPropertyInfo == null)
            {
                return new ValidationResult("failed " + OtherProperty);
            }

            object otherPropertyValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);
            if (!Equals(value, otherPropertyValue))
            {
                if (OtherPropertyDisplayName == null)
                {
                    OtherPropertyDisplayName = GetDisplayNameForProperty(validationContext.ObjectType, OtherProperty);
                }
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            return null;
        }

        private static string GetDisplayNameForProperty(Type containerType, string propertyName)
        {
            ICustomTypeDescriptor typeDescriptor = GetTypeDescriptor(containerType);
            PropertyDescriptor property = typeDescriptor.GetProperties().Find(propertyName, true);
            if (property == null)
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Property not found {0} {1}", containerType.FullName, propertyName));
            }
            IEnumerable<Attribute> attributes = property.Attributes.Cast<Attribute>().ToArray();
            DisplayAttribute display = attributes.OfType<DisplayAttribute>().FirstOrDefault();
            if (display != null)
            {
                return display.GetName();
            }
            DisplayNameAttribute displayName = attributes.OfType<DisplayNameAttribute>().FirstOrDefault();
            if (displayName != null)
            {
                return displayName.DisplayName;
            }
            return propertyName;
        }

        private static ICustomTypeDescriptor GetTypeDescriptor(Type type)
        {
            return new AssociatedMetadataTypeTypeDescriptionProvider(type).GetTypeDescriptor(type);
        }

    }

}
