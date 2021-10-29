using GameModManager.Services.DataProviders.ModLoader;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GameModManager.Models
{
    /// <summary>
    /// Class describes a single provider to get some modification
    /// </summary>
    public class ModProvider
    {
        /// <summary>
        /// The type of the provider
        /// </summary>
        public Type ProviderType { get; }

        /// <summary>
        /// The name of the provider
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// A lazy object to load the mod loader isntance
        /// </summary>
        private readonly Lazy<IModLoader> modLoader;

        /// <summary>
        /// Create a new instance of this class
        /// </summary>
        /// <param name="providerType">The type of the provider to use</param>
        public ModProvider(Type providerType)
        {
            ProviderType = providerType;
            Name = ConvertToName(ProviderType.Name);
            modLoader = new Lazy<IModLoader>(() =>
            {
                if (ProviderType == null)
                {
                    return default;
                }
                ConstructorInfo emptyConstructor = ProviderType.GetConstructors().Where(c => c.IsPublic && c.GetParameters().Length == 0).FirstOrDefault();
                if (emptyConstructor == null)
                {
                    return default;
                }
                try
                {
                    object? providerInstance = Activator.CreateInstance(ProviderType);
                    if (providerInstance is IModLoader loader)
                    {
                        return loader;
                    }
                }
                catch (Exception)
                {
                    // Instance creation did fail
                }

                return default;
            });
        }

        /// <summary>
        /// Convert class name of the provider to a name which can be displayed
        /// </summary>
        /// <param name="name">The name of the class to convert</param>
        /// <returns>The converted name</returns>
        private string ConvertToName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }
            StringBuilder stringBuilder = new StringBuilder(name[0].ToString().ToUpper(CultureInfo.InvariantCulture));
            for(int i=1; i < name.Length; i++)
            {
                string format = "{0}";
                if (char.IsUpper(name[i]))
                {
                    format = " " + format;
                }
                stringBuilder.Append(string.Format(format, name[i]));
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Get the example text for this specific mod provider
        /// </summary>
        /// <returns>The example text</returns>
        public string GetExampleText()
        {
            return GetClassInstance().ExampleUrl;
        }

        /// <summary>
        /// Get a new instance of this mod provider
        /// </summary>
        /// <returns>A useable mod provider instance</returns>
        public IModLoader GetClassInstance()
        {
            return modLoader.Value;
        }
    }
}
