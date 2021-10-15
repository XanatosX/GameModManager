using System;
using System.Diagnostics;

namespace GameModManager.Services.Container
{
    /// <summary>
    /// Class to open url's into the system default browser
    /// </summary>
    public class UrlOpener
    {
        /// <summary>
        /// The url to open
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// The startup information as a lazy class
        /// </summary>
        private readonly Lazy<ProcessStartInfo> processStartInfo;

        /// <summary>
        /// Create a new instance of this class
        /// </summary>
        /// <param name="url">The url to open</param>
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
        
        /// <summary>
        /// Open the url in the browser
        /// </summary>
        public void OpenInBrowser()
        {
            Process.Start(processStartInfo.Value);
        }

        
    }
}
