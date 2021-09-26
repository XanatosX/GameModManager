using System;
using System.Security;

namespace GameModManager.Services.Container
{
    internal class Credentials : ICredentials
    {
        public bool IsDataSet { get; private set; }

        private readonly SecureString username;
        private readonly SecureString password;

        public Credentials(SecureString token)
            :this(new SecureString(), token)
        {
        }

        public Credentials(SecureString username, SecureString password)
        {
            this.username = username;
            this.password = password;

            IsDataSet = true;
        }

        public SecureString GetUserName()
        {
            return IsDataSet ? username : new SecureString();
        }

        public SecureString GetCredentials()
        {
            return IsDataSet ? password : new SecureString();
        }

        public void Dispose()
        {
            password.Dispose();
            username.Dispose();
            IsDataSet = false;
        }
    }
}
