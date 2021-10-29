using System;
using System.Security;

namespace GameModManager.Services.Container
{
    /// <summary>
    /// Class to store credentials in memory
    /// </summary>
    internal class Credentials : ICredentials
    {
        /// <summary>
        /// Is there some data set on this credential instance
        /// </summary>
        public bool IsDataSet { get; private set; }

        /// <summary>
        /// The username to use, could be empty if credentials is a OAuth key
        /// </summary>
        private readonly SecureString username;

        /// <summary>
        /// The password or key to use
        /// </summary>
        private readonly SecureString password;

        /// <summary>
        /// Create a new instance of this class
        /// </summary>
        /// <param name="token">Token to store as a password</param>
        public Credentials(SecureString token)
            :this(new SecureString(), token)
        {
        }

        /// <summary>
        /// Create a new instance of this class
        /// </summary>
        /// <param name="username">Username to store</param>
        /// <param name="password">Password to store</param>
        public Credentials(SecureString username, SecureString password)
        {
            this.username = username;
            this.password = password;

            IsDataSet = true;
        }

        /// <summary>
        /// Get the username or an empty one if nothing set
        /// </summary>
        /// <returns>A secure string with the username</returns>
        public SecureString GetUserName()
        {
            return IsDataSet ? username : new SecureString();
        }

        /// <summary>
        /// Get the password or token, will return an empty one if nothing set
        /// </summary>
        /// <returns>The password or token</returns>
        public SecureString GetCredentials()
        {
            return IsDataSet ? password : new SecureString();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            password.Dispose();
            username.Dispose();
            IsDataSet = false;
        }
    }
}
