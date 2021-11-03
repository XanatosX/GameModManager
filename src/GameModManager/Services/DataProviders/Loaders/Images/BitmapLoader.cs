using System;
using System.Drawing;
using System.IO;

namespace GameModManager.Services.DataProviders.Loaders.Images
{
    /// <summary>
    /// This class will load a bitmap from the hdd
    /// </summary>
    internal class BitmapLoader : AbstractDataLoader<Bitmap>
    {
        /// <inheritdoc/>
        public override Bitmap LoadData(string dataSource)
        {
            if (!File.Exists(dataSource))
            {
                return null;
            }
            try
            {
                return new Bitmap(dataSource);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
