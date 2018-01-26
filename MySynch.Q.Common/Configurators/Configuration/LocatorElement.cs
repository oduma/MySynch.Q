using System.Configuration;

namespace MySynch.Q.Common.Configurators.Configuration
{
    public class LocatorElement : ConfigurationElement
    {
        [ConfigurationProperty("location", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string Location
        {
            get { return (string)this["location"]; }
            set { this["location"] = value; }
        }

        [ConfigurationProperty("serviceName", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string ServiceName
        {
            get { return (string)this["serviceName"]; }
            set { this["serviceName"] = value; }
        }

        [ConfigurationProperty("sectionId", DefaultValue = "sendersSection", IsKey = true, IsRequired = true)]
        public string SectionId
        {
            get { return (string)this["sectionId"]; }
            set { this["sectionId"] = value; }
        }
    }
}