using System.Configuration;

namespace MySynch.Q.Common.Configuration
{
    public class BaseConfigurationSection: ConfigurationSection
    {
        [ConfigurationProperty("localRootFolder", DefaultValue = "", IsRequired = true)]
        public string LocalRootFolder
        {
            get
            {
                return (string)this["localRootFolder"];
            }
            set
            {
                this["localRootFolder"] = value;
            }
        }

    }
}
