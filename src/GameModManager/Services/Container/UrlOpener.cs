using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModManager.Services.Container
{
    public class UrlOpener
    {
        public string Url { get; }

        private readonly Lazy<ProcessStartInfo> processStartInfo;

        public UrlOpener(string url)
        {
            Url = url;
            processStartInfo = new Lazy<ProcessStartInfo>(() =>
            {
                return new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = url
                };
            });
        }

        public void OpenInBrowser()
        {
            Process.Start(processStartInfo.Value);
        }

        
    }
}
