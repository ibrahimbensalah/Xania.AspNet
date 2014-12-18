using System.Security.Principal;

namespace Xania.AspNet.Simulator
{
    public class ActionInfo
    {
        public class Builder
        {
            private readonly ActionInfo _actionInfo;

            public Builder(ActionInfo actionInfo)
            {
                _actionInfo = actionInfo;
            }

            public Builder Get()
            {
                _actionInfo.HttpMethod = "GET";
                return this;
            }

            public Builder Post()
            {
                _actionInfo.HttpMethod = "POST";
                return this;
            }

            public Builder User(string userName, string[] roles, string identityType = "Simulator")
            {
                _actionInfo.User = new GenericPrincipal(new GenericIdentity(userName, identityType), roles ?? new string[] {});
                return this;
            }
        }

        public GenericPrincipal User { get; set; }

        public string HttpMethod { get; set; }
    }
}