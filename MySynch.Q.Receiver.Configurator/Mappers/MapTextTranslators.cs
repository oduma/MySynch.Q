using System.Collections.ObjectModel;
using System.Xml;
using MySynch.Q.Common.Mappers;
using MySynch.Q.Receiver.Configurator.MVVM;

namespace MySynch.Q.Receiver.Configurator.Mappers
{
    internal class MapTextTranslators : IMap<XmlElement, ObservableCollection<TranslatorConfigurationViewModel>>
    {
        private IMap<XmlElement, TranslatorConfigurationViewModel> _mapTrextTranslator;

        public MapTextTranslators(IMap<XmlElement, TranslatorConfigurationViewModel> mapTrextTranslator)
        {
            this._mapTrextTranslator = mapTrextTranslator;
        }

        public ObservableCollection<TranslatorConfigurationViewModel> Map(XmlElement input)
        {
            throw new System.NotImplementedException();
        }

        public XmlElement UnMap(ObservableCollection<TranslatorConfigurationViewModel> input, XmlElement parrentElement)
        {
            throw new System.NotImplementedException();
        }
    }
}