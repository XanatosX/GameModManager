using GameModManager.Services.DataProviders.ModLoader;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameModManager.Models
{
    public class ModProvider
    {
        public Type ProviderType { get; }
        public string Name { get; }
        private readonly Lazy<IModLoader> modLoader;

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
                    return null;
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

                return null;
            });
        }

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

        public string GetExampleText()
        {
            return GetClassInstance().ExampleUrl;
        }

        public IModLoader GetClassInstance()
        {
            return modLoader.Value;
        }
    }
}
