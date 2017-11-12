using System.Configuration;

namespace MySynch.Q.Sender.Configuration
{
    public class SenderElement:ConfigurationElement
    {
        [ConfigurationProperty("minMem", DefaultValue = "", IsRequired = true)]
        public string MinFreeMemory
        {
            get { return (string)this["minMem"]; }
            set { this["minMem"] = value; }
        }

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

        [ConfigurationProperty("extensions")]
        public ExtensionElementCollection Extensions
        {
            get { return (ExtensionElementCollection)this["extensions"]; }
        }

        [ConfigurationProperty("queues")]
        public QueueElementCollection Queues
        {
            get { return (QueueElementCollection)this["queues"]; }
        }

    }
}