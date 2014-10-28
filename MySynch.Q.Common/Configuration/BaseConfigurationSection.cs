using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
