using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySynch.Q.Sender
{
    
    public class SenderSection:ConfigurationSection
    {
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


    }
 
}
