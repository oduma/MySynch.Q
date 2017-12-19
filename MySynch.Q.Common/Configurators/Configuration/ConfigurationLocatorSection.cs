using System.Configuration;

namespace MySynch.Q.Common.Configurators.Configuration
{
    public class ConfigurationLocatorSection : ConfigurationSection
    {
        [ConfigurationProperty("locators")]
        public LocatorElementCollection Locators
        {
            get { return (LocatorElementCollection)this["locators"]; }
        }
    }
}
