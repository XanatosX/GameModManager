using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModManager.Services.DataProviders.Loaders
{
    public interface IDataLoader<T>
    {
        T LoadData(string dataSource);
    }
}
