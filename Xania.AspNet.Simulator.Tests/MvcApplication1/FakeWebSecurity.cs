using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using MvcApplication1.Controllers;
using MvcApplication1.Data;

namespace Xania.AspNet.Simulator.Tests.MvcApplication1
{
    public class FakeWebSecurity : IWebSecurity
    {
        private readonly ICollection<ApplicationUser> _users;

        public IPrincipal CurrentUser { get; private set; }

        public FakeWebSecurity(ICollection<ApplicationUser> users)
        {
            _users = users;
        }

        public bool Login(string userName, string password, bool persistCookie = true)
        {
            CurrentUser = new GenericPrincipal(new GenericIdentity(userName ?? string.Empty, "simulator"), new string[0]); ;
            return true;
        }

        public void Logout()
        {
            CurrentUser = new GenericPrincipal(new GenericIdentity(string.Empty, "simulator"), new string[0]); ;
        }

        public void CreateUserAndAccount(string userName, string password)
        {
            _users.Add(new ApplicationUser()
            {
                UserId = _users.Any() ? _users.Max(e => e.UserId) + 1 : 1,
                UserName = userName,
                Password = password
            });
        }

        public int GetUserId(string userName)
        {
            var user = _users.Single(u => u.UserName.Equals(userName, StringComparison.InvariantCulture));
            return user.UserId;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            throw new System.NotImplementedException();
        }

        public void CreateAccount(string userName, string newPassword)
        {
            throw new System.NotImplementedException();
        }
    }
}