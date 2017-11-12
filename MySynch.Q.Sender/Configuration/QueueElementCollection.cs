using System.Configuration;

namespace MySynch.Q.Sender.Configuration
{
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

}