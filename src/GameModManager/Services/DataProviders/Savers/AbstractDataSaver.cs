using System.Threading.Tasks;

namespace GameModManager.Services.DataProviders.Savers
{
    /// <summary>
    /// Abstract data saver class
    /// </summary>
    /// <typeparam name="T">The type the saver should save</typeparam>
    public abstract class AbstractDataSaver<T> : IDataSaver<T>
    {
        /// <inheritdoc/>
        public abstract bool SaveData(T data, string connectionString);

        /// <inheritdoc/>
        public async Task<bool> SaveDataAsync(T data, string connectionString)
        {
            return await Task.Run(() => SaveData(data, connectionString));
        }
    }
}
