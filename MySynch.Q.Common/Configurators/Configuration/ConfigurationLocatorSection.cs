using System.Configuration;

namespace MySynch.Q.Common.Configurators.Configuration
{
    public class ConfigurationLocatorSection : ConfigurationSection
    {
        [ConfigurationProperty("locators")]
        public LocatorElementCollection Locators => (LocatorElementCollection)this["locators"];
    }
}
