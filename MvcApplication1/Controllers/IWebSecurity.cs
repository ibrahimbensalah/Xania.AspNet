using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using MvcApplication1.Data;

namespace MvcApplication1.Controllers
{
    public interface IWebSecurity
    {
        bool Login(string userName, string password, bool persistCookie = true);
        void Logout();
        void CreateUserAndAccount(string userName, string password);
        int GetUserId(string userName);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
        void CreateAccount(string userName, string newPassword);
    }
}