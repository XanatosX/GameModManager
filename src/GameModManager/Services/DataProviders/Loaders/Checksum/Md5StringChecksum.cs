using System.Security.Cryptography;
using System.Text;

namespace GameModManager.Services.DataProviders.Loaders.Checksum
{
    /// <summary>
    /// This class will convert the input string to an md5 hash
    /// </summary>
    internal class Md5StringChecksum : AbstractDataLoader<string>
    {
        /// <inheritdoc/>
        public override string LoadData(string dataSource)
        {
            StringBuilder builder = new StringBuilder();
            using (MD5 mD5 = MD5.Create())
            {
                byte[] hash = mD5.ComputeHash(Encoding.ASCII.GetBytes(dataSource));
                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("X2"));
                }
            }
            return builder.ToString();
        }

    }
}
