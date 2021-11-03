using System.Threading.Tasks;

namespace GameModManager.Services.DataProviders.Loaders
{
    /// <summary>
    /// Abstract class to load data of type T
    /// </summary>
    /// <typeparam name="T">The data type the loader should load</typeparam>
    public abstract class AbstractDataLoader<T> : IDataLoader<T>
    {
        /// <inheritdoc/>
        public abstract T LoadData(string dataSource);

        /// <inheritdoc/>
        public async Task<T> LoadDataAsync(string dataSource)
        {
            return await Task.Run(() => LoadData(dataSource));
        }
    }
}
