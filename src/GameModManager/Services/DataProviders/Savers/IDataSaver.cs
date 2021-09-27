using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModManager.Services.DataProviders.Savers
{
    public interface IDataSaver<in T>
    {
        bool SaveData(T data, string file);
    }
}
