using GameModManager.Models;
using GameModManager.Services.DataProviders.ModLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModManager.Services.DataProviders.Loaders.Mod
{
    public class ModLoaderProvider : IDataLoader<IReadOnlyList<ModProvider>>
    {
        public IReadOnlyList<ModProvider> LoadData(string dataSource)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes())
                                                          .Where(t => t.IsClass && t.Namespace == dataSource)
                                                          .Where(t => t.GetInterfaces().Contains(typeof(IModLoader)))
                                                          .Select(t => new ModProvider(t))
                                                          .ToList()
                                                          .AsReadOnly();
        }
    }
}
