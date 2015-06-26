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

    public class WebSecurity : IWebSecurity
    {
        public bool Login(string userName, string password, bool persistCookie = true)
        {
            throw new System.NotImplementedException();
        }

        public void Logout()
        {
            throw new System.NotImplementedException();
        }

        public void CreateUserAndAccount(string userName, string password)
        {
            throw new System.NotImplementedException();
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