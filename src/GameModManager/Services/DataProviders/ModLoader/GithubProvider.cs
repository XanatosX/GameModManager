using GameModManager.Models;
using GameModManager.Services.Container;
using GameModManager.Services.DataProviders.Savers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GameModManager.Services.DataProviders.ModLoader
{
    public class GithubProvider : IModLoader
    {
        public string ExampleUrl => "https://github.com/XanatosX/GameModManager";

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

        private string[] SplitUrl(string url)
        {
            return url.Split("/").Where(s => !string.IsNullOrEmpty(s)).ToArray();
        }

        private string GetUsernameFromUrl(string url)
        {
            return GetUsernameFromUrl(SplitUrl(url));
        }

        private string GetUsernameFromUrl(string[] splittedUrl)
        {
            if (splittedUrl.Length < 3)
            {
                return string.Empty;
            }
            return splittedUrl[2];
        }

        private string GetProjectFromUrl(string url)
        {
            return GetProjectFromUrl(SplitUrl(url));
        }

        private string GetProjectFromUrl(string[] splittedUrl)
        {
            if (splittedUrl.Length < 4)
            {
                return string.Empty;
            }
            return splittedUrl[3];
        }

        public Version GetLatestReleaseVersion(Game gameData)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DownloadLatestRelease(Game gameData, Container.ICredentials credentials, IDataSaver<ReleaseArtifact> dataSaver)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InstallLatestPatch(Game gameData, string cacheFolder)
        {
            throw new NotImplementedException();
        }
    }
}
