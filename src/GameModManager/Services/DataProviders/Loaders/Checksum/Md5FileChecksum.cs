using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GameModManager.Services.DataProviders.Loaders.Checksum
{
    public class Md5FileChecksum : IDataLoader<string>
    {
        public string LoadData(string dataSource)
        {
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
