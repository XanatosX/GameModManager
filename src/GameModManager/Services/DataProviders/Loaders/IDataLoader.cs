using System.Threading.Tasks;

namespace GameModManager.Services.DataProviders.Loaders
{
    /// <summary>
    /// Interface to load data from different sources
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataLoader<T>
    {
        /// <summary>
        /// Load the data from the given data source
        /// </summary>
        /// <param name="dataSource">The data source or connection string to load the data from</param>
        /// <returns>A valid dataset of the given type T</returns>
        T LoadData(string dataSource);

        /// <summary>
        /// Async task to load the data from the given data source
        /// </summary>
        /// <param name="dataSource">The data source or conenction string to load the data from</param>
        /// <returns>A valid dataset of the given type T</returns>
        Task<T> LoadDataAsync(string dataSource);
    }
}
