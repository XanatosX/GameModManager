using GameModManager.Services.Container;
using GameModManager.Services.DataProviders.Loaders;
using GameModManager.Services.DataProviders.Loaders.Checksum;
using GameModManager.Services.DataProviders.Loaders.Images;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace GameModManager.Models
{
 
    /// <summary>
    /// Class to represent a game to update it's mods
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Path to the exe of the game to get the image from
        /// </summary>
        public string GameExe { get; private set; }

        /// <summary>
        /// The folder to place the downloaded files into
        /// </summary>
        public string TargetFolderPath { get; }

        /// <summary>
        /// The remote url to get the mod from
        /// </summary>
        public string RemoteUrl => url.Value.Url;

        /// <summary>
        /// The remote url to get the mod from
        /// </summary>
        private readonly Lazy<UrlOpener> url;

        /// <summary>
        /// The provider to use for loading the mod and updating the client
        /// </summary>
        public ModProvider DataProviderToUse { get; }

        /// <summary>
        /// The name of the game you are updating
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Last time the game was synced with the remote source
        /// </summary>
        public DateTime LastUpdateTime { get; private set; }

        /// <summary>
        /// The current locally installed version
        /// </summary>
        public Version LocalVersion { get; private set; }

        /// <summary>
        /// The current remote installed version
        /// </summary>
        public Version RemoteVersion { get; private set; }

        /// <summary>
        /// Cached image file to use
        /// </summary>
        public String CachedImageFile { get; private set; }

        /// <summary>
        /// Create a new instance of the game class
        /// </summary>
        /// <param name="gameExe">The game exe to get the image from</param>
        /// <param name="name">The name of the game to show</param>
        /// <param name="targetFolderPath">The folder to place the download in</param>
        /// <param name="remoteUrl">The remote url to get the data from</param>
        /// <param name="dataProviderToUse">The data provider to use</param>
        public Game(string gameExe, string name, string targetFolderPath, string remoteUrl, ModProvider dataProviderToUse)
        {
            Name = name;
            GameExe = gameExe;
            TargetFolderPath = targetFolderPath;
            url = new Lazy<UrlOpener>(() => new UrlOpener(remoteUrl));
            DataProviderToUse = dataProviderToUse;

            LocalVersion = new Version(0, 0, 0);
            RemoteVersion = new Version(0, 0, 0);
        }

        /// <summary>
        /// Open the link in the browser
        /// </summary>
        /// <returns>Task you can await which will be done as soon as it was opened in the browser</returns>
        public async Task OpenLinkInBrowser()
        {
            await Task.Run(() => url.Value.OpenInBrowser()).ConfigureAwait(false);
        }

        /// <summary>
        /// Load the game image and return the data stream
        /// </summary>
        /// <returns>A stream with the game image</returns>
        public async Task<Stream> LoadGameImage()
        {
            return await Task<Stream>.Run(() =>
            {
                MemoryStream returnStream = new MemoryStream();
                if (!File.Exists(GameExe))
                {
                    returnStream.Close();
                    return returnStream;
                }
                if (File.Exists(GetCachedFileName()))
                {
                    IDataLoader<Bitmap> dataLoader = new BitmapLoader();
                    Bitmap image = dataLoader.LoadData(GetCachedFileName());
                    image.Save(returnStream, ImageFormat.Png);
                    returnStream.Position = 0;
                    return returnStream;
                }
                Icon loadedIcon = Icon.ExtractAssociatedIcon(GameExe);
                loadedIcon.ToBitmap().Save(returnStream, ImageFormat.Png);
                SaveChachedImage(loadedIcon.ToBitmap());
                returnStream.Position = 0;
                return returnStream;
            });
        }

        /// <summary>
        /// Get the name of the cached image file
        /// </summary>
        /// <returns>The name of the file</returns>
        private String GetCachedFileName()
        {
            IDataLoader<string> dataLoader = new Md5StringChecksum();
            string fileName = String.Format("{0}.bmp", dataLoader.LoadData(string.Format("{0}{1}", GameExe, RemoteUrl)));
            return Path.Combine(Settings.Instance.CachePath, fileName);
        }

        /// <summary>
        /// Save the image to the cache to stop loading it from the game exe
        /// </summary>
        /// <param name="imageToSave">The image to save</param>
        /// <returns>A awaitable task</returns>
        public async Task SaveChachedImage(Bitmap imageToSave)
        {
            await Task.Run(() =>
            {
                if (!Directory.Exists(Settings.Instance.CachePath))
                {
                    Directory.CreateDirectory(Settings.Instance.CachePath);
                }
                string cacheFileName = GetCachedFileName();
                imageToSave.Save(cacheFileName);
            });
        }
    }
}
