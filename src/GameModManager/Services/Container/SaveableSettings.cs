using GameModManager.Models;

namespace GameModManager.Services.Container
{
    /// <summary>
    /// Class to represent a saveable setting instance
    /// </summary>
    internal class SaveableSettings
    {
        /// <summary>
        /// The Cache folder to use
        /// </summary>
        public string CachePath { get; set; }

        /// <summary>
        /// Create a new instance of this class and prefill with data from the settings
        /// </summary>
        /// <param name="settings">The settings to save</param>
        public SaveableSettings(Settings settings)
        {
            CachePath = settings.CachePath;
        }

        /// <summary>
        /// Create a new instance of this class
        /// </summary>
        public SaveableSettings()
        {
        }
    }
}
