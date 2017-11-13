using System.Configuration;

namespace MySynch.Q.Receiver
{
    public class ReceiverElementCollection:ConfigurationElementCollection
    {
        public ReceiverElement this[int index]
        {
            get { return (ReceiverElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new ReceiverElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ReceiverElement)element).Name;
        }
    }
}