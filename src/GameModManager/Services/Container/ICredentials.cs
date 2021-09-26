using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace GameModManager.Services.Container
{
    public interface ICredentials : IDisposable
    {
        bool IsDataSet { get; }

        SecureString GetUserName();

        SecureString GetCredentials();
    }
}
