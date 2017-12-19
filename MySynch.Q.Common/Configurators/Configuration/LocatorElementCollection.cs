using System.Configuration;

namespace MySynch.Q.Common.Configurators.Configuration
{
    [ConfigurationCollection(typeof(LocatorElement))]
    public class LocatorElementCollection : ConfigurationElementCollection
    {
        public LocatorElement this[int index]
        {
            get { return (LocatorElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new LocatorElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LocatorElement)element).SectionId;
        }
    }
}