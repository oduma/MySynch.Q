﻿using System.Configuration;

namespace MySynch.Q.Sender.Configuration
{

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