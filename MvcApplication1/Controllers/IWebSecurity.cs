using System;
using System.Collections;
using System.Collections.Generic;
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

    public class WebSecurityImpl : IWebSecurity
    {
        private readonly HttpContextBase _httpContext;
        private readonly ICollection<ApplicationUser> _users;

        public WebSecurityImpl(HttpContextBase httpContext, ICollection<Data.ApplicationUser> users)
        {
            _httpContext = httpContext;
            _users = users;
        }

        public bool Login(string userName, string password, bool persistCookie = true)
        {
            var httpCookie = new HttpCookie("__AUTH", userName);
            _httpContext.Response.Cookies.Add(httpCookie);

            return true;
        }

        public void Logout()
        {
            var httpCookie = new HttpCookie("__AUTH", _httpContext.User.Identity.Name)
            {
                Expires = DateTime.Today
            };

            _httpContext.Response.Cookies.Add(httpCookie);
        }

        public void CreateUserAndAccount(string userName, string password)
        {
            _users.Add(new ApplicationUser()
            {
                UserName = userName,
                Password = password
            });
        }

        public int GetUserId(string userName)
        {
            throw new System.NotImplementedException();
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