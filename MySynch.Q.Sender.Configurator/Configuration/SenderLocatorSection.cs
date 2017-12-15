using System.Configuration;

namespace MySynch.Q.Sender.Configurator.Configuration
{
    public class SenderLocatorSection : ConfigurationSection
    {
        [ConfigurationProperty("location", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Location
        {
            get { return (string)this["location"]; }
            set { this["location"] = value; }
        }

        [ConfigurationProperty("sectionId", DefaultValue = "sendersSection", IsKey = false, IsRequired = true)]
        public string SectionId
        {
            get { return (string) this["sectionId"]; }
            set { this["sectionId"] = value; }
        }
    }
}
