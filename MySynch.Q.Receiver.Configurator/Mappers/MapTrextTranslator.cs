using System.Xml;
using MySynch.Q.Common.Mappers;
using MySynch.Q.Receiver.Configurator.MVVM;

namespace MySynch.Q.Receiver.Configurator.Mappers
{
    public class MapTrextTranslator : IMap<XmlElement, TranslatorConfigurationViewModel>
    {
        public TranslatorConfigurationViewModel Map(XmlElement input)
        {
            throw new System.NotImplementedException();
        }

        public XmlElement UnMap(TranslatorConfigurationViewModel input, XmlElement parrentElement)
        {
            throw new System.NotImplementedException();
        }
    }
}