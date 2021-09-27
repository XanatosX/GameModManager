using GameModManager.Services.Container;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModManager.Models
{
    public class Game
    {
        public string GameExe { get; private set; }

        public string TargetFolderPath { get; }

        public string RemoteUrl => url.Value.Url;

        private readonly Lazy<UrlOpener> url;
        public ModProvider DataProviderToUse { get; }
        public string Name { get; private set; }

        public DateTime LastUpdateTime { get; private set; }

        public Version LocalVersion { get; private set; }

        public Version RemoteVersion { get; private set; }

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

        public async Task OpenLinkInBrowser()
        {
            await Task.Run(() => url.Value.OpenInBrowser()).ConfigureAwait(false);
        }

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
                Icon loadedIcon = Icon.ExtractAssociatedIcon(GameExe);
                loadedIcon.ToBitmap().Save(returnStream, ImageFormat.Png);
                returnStream.Position = 0;
                return returnStream;
            });

        }
    }
}
