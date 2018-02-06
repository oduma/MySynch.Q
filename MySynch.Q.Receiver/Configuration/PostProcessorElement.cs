using System.Configuration;

namespace MySynch.Q.Receiver.Configuration
{
    public class PostProcessorElement : ConfigurationElement
    {
        [ConfigurationProperty("name", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("value", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string Value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }

    }
}