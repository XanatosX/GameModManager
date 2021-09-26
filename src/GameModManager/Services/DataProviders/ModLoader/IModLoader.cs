using GameModManager.Models;
using GameModManager.Services.Container;
using GameModManager.Services.DataProviders.Savers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModManager.Services.DataProviders.ModLoader
{
    public interface IModLoader
    {
        string ExampleUrl { get; }

        bool CheckUrlIsValid(string url);

        Version GetLatestReleaseVersion(Game gameData);

        Task<bool> DownloadLatestRelease(Game gameData, ICredentials credentials, IDataSaver<ReleaseArtifact> dataSaver);

        Task<bool> InstallLatestPatch(Game gameData, string cacheFolder);
    }
}
