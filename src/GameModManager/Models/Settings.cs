using GameModManager.Services.Container;
using GameModManager.Services.DataProviders.Loaders;
using GameModManager.Services.DataProviders.Savers;
using System;
using System.IO;
using System.Text.Json.Serialization;

namespace GameModManager.Models
{
    /// <summary>
    /// Singelton class model to store all the settings
    /// </summary>
    internal class Settings
    {
        /// <summary>
        /// The instance to use
        /// </summary>
        private static Settings instance;

        /// <summary>
        /// The padlock to make this class thread save
        /// </summary>
        private static readonly object padlock = new object();

        /// <summary>
        /// The base path for all the settings
        /// </summary>
        [JsonIgnore]
        public string BaseSettingPath => baseSettingsPath;

        /// <summary>
        /// Private access to the base path for the settings
        /// </summary>
        private readonly string baseSettingsPath;

        /// <summary>
        /// The Path to the settings file
        /// </summary>
        [JsonIgnore]
        public string SettingFilePath => settingFilePath;

        /// <summary>
        /// Private access to the settings file path
        /// </summary>
        private readonly string settingFilePath;
        
        /// <summary>
        /// Path to the chache folder
        /// </summary>
        public string CachePath => cachePath;

        /// <summary>
        /// Private access to the path folder
        /// </summary>
        private string cachePath;

        /// <summary>
        /// Loader to use for loading the settings
        /// </summary>
        private Lazy<IDataLoader<SaveableSettings>> loader;

        /// <summary>
        /// Saver to use for saving the settings
        /// </summary>
        private Lazy<IDataSaver<SaveableSettings>> saver;

        /// <summary>
        /// Private constructor to prevent creating class instances from external
        /// </summary>
        private Settings()
        {
            loader = new Lazy<IDataLoader<SaveableSettings>>(new JsonDataLoader<SaveableSettings>());
            saver = new Lazy<IDataSaver<SaveableSettings>>(new JsonDataSaver<SaveableSettings>());
            baseSettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            baseSettingsPath = Path.Combine(baseSettingsPath, "Game Mod Manager");
            settingFilePath = Path.Combine(baseSettingsPath, "Settings.set");

            Load();
            cachePath = String.IsNullOrEmpty(cachePath) ? Path.Combine(baseSettingsPath, "cache") : cachePath;
        }

        /// <summary>
        /// Get a new instance of this singelton class
        /// </summary>
        public static Settings Instance
        {
            get
            {
                lock (padlock)
                {
                    return instance ?? new Settings();
                }
            }
        }

        /// <summary>
        /// Load the data from the disc
        /// </summary>
        /// <returns>True if loading was successful</returns>
        public bool Load()
        {
            if (!File.Exists(settingFilePath))
            {
                return false;
            }
            SaveableSettings data = loader.Value.LoadData(settingFilePath);
            return true;
        }

        /// <summary>
        /// Save the data to the disc
        /// </summary>
        /// <returns>True if saving was successful</returns>
        public bool Save()
        {
            return saver.Value.SaveData(new SaveableSettings(this), settingFilePath);
        }

        public void Delete()
        {

        }
    }
}
