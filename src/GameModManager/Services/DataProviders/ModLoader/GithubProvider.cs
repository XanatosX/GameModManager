using GameModManager.Models;
using GameModManager.Services.Container;
using GameModManager.Services.DataProviders.Savers;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace GameModManager.Services.DataProviders.ModLoader
{
    /// <summary>
    /// Class to get the data from GitHub
    /// </summary>
    public class GithubProvider : IModLoader
    {
        /// <inheritdoc/>
        public string ExampleUrl => "https://github.com/XanatosX/GameModManager";

        /// <inheritdoc/>
        public bool CheckUrlIsValid(string url)
        {
            string[] splitted = SplitUrl(url);
            if (splitted.Length < 4)
            {
                return false;
            }
            bool valid = true;
            valid &= splitted[1].ToLower(CultureInfo.InvariantCulture) == "github.com";
            valid &= !string.IsNullOrEmpty(GetUsernameFromUrl(splitted));
            valid &= !string.IsNullOrEmpty(GetProjectFromUrl(splitted));
            return valid;
        }

        /// <summary>
        /// Split the url
        /// </summary>
        /// <param name="url">The url to split</param>
        /// <returns>The splitted url</returns>
        private string[] SplitUrl(string url)
        {
            return url.Split("/").Where(s => !string.IsNullOrEmpty(s)).ToArray();
        }

        /// <summary>
        /// Get the username from the given url
        /// </summary>
        /// <param name="url">The url to extract the username from</param>
        /// <returns>A valid username if possible</returns>
        private string GetUsernameFromUrl(string url)
        {
            return GetUsernameFromUrl(SplitUrl(url));
        }

        /// <summary>
        /// Get the username from the given url
        /// </summary>
        /// <param name="splittedUrl">The already splitted url to get the username from</param>
        /// <returns>A valid username if possible</returns>
        private string GetUsernameFromUrl(string[] splittedUrl)
        {
            if (splittedUrl.Length < 3)
            {
                return string.Empty;
            }
            return splittedUrl[2];
        }

        /// <summary>
        /// Get the project from the given url
        /// </summary>
        /// <param name="url">The url to extract the project from</param>
        /// <returns>A valid project name if possible</returns>
        private string GetProjectFromUrl(string url)
        {
            return GetProjectFromUrl(SplitUrl(url));
        }

        /// <summary>
        /// Get the project from the given url
        /// </summary>
        /// <param name="splittedUrl">The already splitted url to extract the project from</param>
        /// <returns>A valid project name if possible</returns>
        private string GetProjectFromUrl(string[] splittedUrl)
        {
            if (splittedUrl.Length < 4)
            {
                return string.Empty;
            }
            return splittedUrl[3];
        }

        /// <inheritdoc/>
        public Version GetLatestReleaseVersion(Game gameData)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> DownloadLatestRelease(Game gameData, Container.ICredentials credentials, IDataSaver<ReleaseArtifact> dataSaver)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> InstallLatestPatch(Game gameData, string cacheFolder)
        {
            throw new NotImplementedException();
        }
    }
}
