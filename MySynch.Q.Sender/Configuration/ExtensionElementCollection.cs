﻿using System.Configuration;

namespace MySynch.Q.Sender.Configuration
{
    [ConfigurationCollection(typeof(ExtensionElement))]
    public class ExtensionElementCollection : ConfigurationElementCollection
    {
        public ExtensionElement this[int index]
        {
            get { return (ExtensionElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ExtensionElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ExtensionElement)element).Key;
        }
    }

}