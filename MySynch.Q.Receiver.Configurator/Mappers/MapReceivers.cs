using System.Collections.ObjectModel;
using System.Xml;
using MySynch.Q.Common.Mappers;
using MySynch.Q.Receiver.Configurator.MVVM;

namespace MySynch.Q.Receiver.Configurator.Mappers
{
    public class MapReceivers : IMap<XmlElement, ObservableCollection<ReceiverConfigurationViewModel>>
    {
        private IMap<XmlElement, ReceiverConfigurationViewModel> _mapReceiver;

        public MapReceivers(IMap<XmlElement, ReceiverConfigurationViewModel> mapReceiver)
        {
            this._mapReceiver = mapReceiver;
        }

        public ObservableCollection<ReceiverConfigurationViewModel> Map(XmlElement input)
        {
            throw new System.NotImplementedException();
        }

        public XmlElement UnMap(ObservableCollection<ReceiverConfigurationViewModel> input, XmlElement parrentElement)
        {
            throw new System.NotImplementedException();
        }
    }
}