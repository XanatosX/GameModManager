namespace GameModManager.Services.DataProviders.Savers
{
    /// <summary>
    /// Interface to define a loading class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataSaver<in T>
    {
        /// <summary>
        /// Save the dataset to a connection string
        /// </summary>
        /// <param name="data">The data which should be save of type T</param>
        /// <param name="connectionString">The connection string used to save the data set to</param>
        /// <returns>True if saving was successful</returns>
        bool SaveData(T data, string connectionString);
    }
}
