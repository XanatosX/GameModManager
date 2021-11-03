using System;
using System.IO;
using System.Text.Json;

namespace GameModManager.Services.DataProviders.Savers
{
    /// <summary>
    /// Class to save an object as json to the hdd
    /// </summary>
    /// <typeparam name="T">The type of data to save</typeparam>
    internal class JsonDataSaver<T> : AbstractDataSaver<T>
    {
        /// <inheritdoc/>
        public override bool SaveData(T data, string connectionString)
        {
            bool returnState = false;
            using (StreamWriter writer = new StreamWriter(connectionString))
            {
                try
                {
                    string tdata = JsonSerializer.Serialize(data);
                    writer.WriteLine(JsonSerializer.Serialize(data));
                    returnState = true;
                }
                catch (Exception)
                {
                    // Something went wrong while trying to save the data
                }
            }

            return returnState;
        }
    }
}
