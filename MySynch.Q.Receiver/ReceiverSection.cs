using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySynch.Q.Receiver
{
    
    public class ReceiverSection:ConfigurationSection
    {
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
