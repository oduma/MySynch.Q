using MySynch.Q.Common.Configuration;
using System.Configuration;

namespace MySynch.Q.Sender
{
    
    public class SenderSection:BaseConfigurationSection
    {
        [ConfigurationProperty("minMem", DefaultValue = "", IsRequired = true)]
        public string MinFreeMemory
        {
            get { return (string)this["minMem"]; }
            set { this["minMem"] = value; }
        }

        [ConfigurationProperty("maxFileSize", DefaultValue = 0, IsRequired = true)]
        public int MaxFileSize
        {
            get { return (int)this["maxFileSize"]; }
            set { this["maxFileSize"] = value; }
        }

        [ConfigurationProperty("queues")]
        public QueueElementCollection Queues
        {
            get { return (QueueElementCollection)this["queues"]; }
        }

    }

    [ConfigurationCollection(typeof(QueueElement))]
    public class QueueElementCollection : ConfigurationElementCollection
    {
        public QueueElement this[int index]
        {
            get { return (QueueElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new QueueElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((QueueElement)element).Name;
        }
    }

    public class QueueElement : ConfigurationElement
    {
        public QueueElement() { }

        [ConfigurationProperty("name", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("queueName", DefaultValue = "", IsRequired = true)]
        public string QueueName
        {
            get { return (string)this["queueName"]; }
            set { this["queueName"] = value; }
        }

        [ConfigurationProperty("hostName", DefaultValue = "", IsRequired = true)]
        public string HostName
        {
            get { return (string)this["hostName"]; }
            set { this["hostName"] = value; }
        }

        [ConfigurationProperty("userName", DefaultValue = "", IsRequired = true)]
        public string UserName
        {
            get { return (string)this["userName"]; }
            set { this["userName"] = value; }
        }
        [ConfigurationProperty("password", DefaultValue = "", IsRequired = true)]
        public string Password
        {
            get { return (string)this["password"]; }
            set { this["password"] = value; }
        }
    }
 
}
