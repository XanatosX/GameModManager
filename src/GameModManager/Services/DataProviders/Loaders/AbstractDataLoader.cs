using System.Threading.Tasks;

namespace GameModManager.Services.DataProviders.Loaders
{
    public abstract class AbstractDataLoader<T> : IDataLoader<T>
    {
        /// <inheritdoc/>
        public abstract T LoadData(string dataSource);

        /// <inheritdoc/>
        public Task<T> LoadDataAsync(string dataSource)
        {
            return Task.Run(() => LoadData(dataSource));
        }
    }
}
