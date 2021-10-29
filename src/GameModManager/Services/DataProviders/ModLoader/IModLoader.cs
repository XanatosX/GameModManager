using GameModManager.Models;
using GameModManager.Services.Container;
using GameModManager.Services.DataProviders.Savers;
using System;
using System.Threading.Tasks;

namespace GameModManager.Services.DataProviders.ModLoader
{
    /// <summary>
    /// Interface to load a mod from a sepcific source in a specific format
    /// </summary>
    public interface IModLoader
    {
        /// <summary>
        /// Example the url how the source should look like
        /// </summary>
        string ExampleUrl { get; }

        /// <summary>
        /// Check if the url used for mod loading is valid
        /// </summary>
        /// <param name="url">The url which should be checked</param>
        /// <returns>True if the url ist valid</returns>
        bool CheckUrlIsValid(string url);

        /// <summary>
        /// Get the latest release version from the remote source
        /// </summary>
        /// <param name="gameData">The data for the game</param>
        /// <returns>The latest remote version</returns>
        Version GetLatestReleaseVersion(Game gameData);

        /// <summary>
        /// Download the latest release from a given source
        /// </summary>
        /// <param name="gameData">The game data to get the release for</param>
        /// <param name="credentials">The credentials to use for authentification on the source</param>
        /// <param name="dataSaver">The data saver to use for saving the artifact</param>
        /// <returns></returns>
        Task<bool> DownloadLatestRelease(Game gameData, ICredentials credentials, IDataSaver<ReleaseArtifact> dataSaver);

        /// <summary>
        /// Install the latest patch from the cache folder
        /// </summary>
        /// <param name="gameData">The game data to use for the installation</param>
        /// <param name="cacheFolder">Path to the chache folder</param>
        /// <returns>True if installing was a success</returns>
        Task<bool> InstallLatestPatch(Game gameData, string cacheFolder);
    }
}
