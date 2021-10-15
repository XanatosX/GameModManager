using GameModManager.Models;
using GameModManager.Services.DataProviders.ModLoader;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameModManager.Services.DataProviders.Loaders.Mod
{
    /// <summary>
    /// Class to get all the mod providers in this project
    /// </summary>
    public class ModLoaderProvider : AbstractDataLoader<IReadOnlyList<ModProvider>>
    {
        /// <inheritdoc/>
        public override IReadOnlyList<ModProvider> LoadData(string dataSource)
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
