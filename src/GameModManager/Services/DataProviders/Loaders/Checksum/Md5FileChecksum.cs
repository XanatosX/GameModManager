using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GameModManager.Services.DataProviders.Loaders.Checksum
{
    /// <summary>
    /// Class to get the checksum for some file content
    /// </summary>
    public class Md5FileChecksum : AbstractDataLoader<string>
    {
        /// <inheritdoc/>
        public override string LoadData(string dataSource)
        {
            if (!File.Exists(dataSource))
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            using (MD5 mD5 = MD5.Create())
            {
                using(FileStream fileStream = new FileStream(dataSource, FileMode.Open))
                {
                    using (BinaryReader stream = new BinaryReader(fileStream))
                    {
                        byte[] hash = mD5.ComputeHash(stream.BaseStream);
                        for (int i = 0; i < hash.Length; i++)
                        {
                            builder.Append(hash[i].ToString("X2"));
                        }
                    }
                }
            }
            return builder.ToString();
        }
    }
}
