using System.Configuration;

namespace MySynch.Q.Receiver.Configuration
{
    public class PostProcessorElementCollection:ConfigurationElementCollection
    {
        public PostProcessorElement this[int index]
        {
            get { return (PostProcessorElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new PostProcessorElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PostProcessorElement)element).Name;
        }
    }
}