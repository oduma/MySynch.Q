using System.Configuration;

namespace MySynch.Q.Sender.Configuration
{
    public class ExtensionElement : ConfigurationElement
    {
        public ExtensionElement()
        {
        }

        [ConfigurationProperty("key", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Key
        {
            get { return (string)this["key"]; }
            set { this["key"] = value; }
        }

        [ConfigurationProperty("value", DefaultValue = "", IsRequired = true)]
        public string Value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }

    }
}