using System.Xml;
using MySynch.Q.Common.Mappers;
using MySynch.Q.Receiver.Configurator.MVVM;

namespace MySynch.Q.Receiver.Configurator.Mappers
{
    internal class MapReceiver : IMap<XmlElement, ReceiverConfigurationViewModel>
    {
        public ReceiverConfigurationViewModel Map(XmlElement input)
        {
            throw new System.NotImplementedException();
        }

        public XmlElement UnMap(ReceiverConfigurationViewModel input, XmlElement parrentElement)
        {
            throw new System.NotImplementedException();
        }
    }
}