using System.Configuration;

namespace MySynch.Q.Sender.Configuration
{
    [ConfigurationCollection(typeof(SenderElement))]
    public class SenderElementCollection: ConfigurationElementCollection
    {
        public SenderElement this[int index]
        {
            get { return (SenderElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new SenderElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SenderElement)element).LocalRootFolder;
        }
    }
}