using System;
using System.IO;
using System.Text.Json;

namespace GameModManager.Services.DataProviders.Loaders
{
    /// <summary>
    /// Class to load a json file from the system
    /// </summary>
    /// <typeparam name="T">The type of data to load</typeparam>
    internal class JsonDataLoader<T> : AbstractDataLoader<T>
    {
        public override T LoadData(string dataSource)
        {
            T returnData = default(T);
            if (!File.Exists(dataSource))
            {
                return returnData;
            }
            
            using (StreamReader reader = new StreamReader(dataSource))
            {
                try
                {
                    returnData = JsonSerializer.Deserialize<T>(reader.ReadToEnd());
                }
                catch (Exception)
                {
                    //Loading failed, whups
                }
            }
            return returnData;
        }
    }
}
