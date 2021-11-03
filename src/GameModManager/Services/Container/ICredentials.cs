using System;
using System.Security;

namespace GameModManager.Services.Container
{
    /// <summary>
    /// Interface for credentials with scope of the application
    /// </summary>
    public interface ICredentials : IDisposable
    {
        /// <summary>
        /// Is there some data set
        /// </summary>
        bool IsDataSet { get; }

        /// <summary>
        /// Get the username for the credentials
        /// </summary>
        /// <returns>The username as a secure string</returns>
        SecureString GetUserName();

        /// <summary>
        /// Get the credentials like the password or oauth key
        /// </summary>
        /// <returns>The credentials as secure string</returns>
        SecureString GetCredentials();
    }
}
