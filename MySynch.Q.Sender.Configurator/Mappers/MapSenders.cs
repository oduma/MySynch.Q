using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MySynch.Q.Common.Mappers;
using MySynch.Q.Sender.Configurator.Configuration;
using MySynch.Q.Sender.Configurator.MVVM;

namespace MySynch.Q.Sender.Configurator.Mappers
{
    public class MapSenders:IMap<XmlElement,ObservableCollection<SenderConfigurationViewModel>>
    {
        private readonly IMap<XmlElement,SenderConfigurationViewModel> _mapSender;

        public MapSenders(
            IMap<XmlElement, SenderConfigurationViewModel> mapSender)
        {
            _mapSender = mapSender;
        }

        public ObservableCollection<SenderConfigurationViewModel> Map(XmlElement input)
        {
            if (_mapSender == null)
                return null;
            if (input == null)
                return null;
            var result = new ObservableCollection<SenderConfigurationViewModel>();

            foreach (var senderNode in input.ChildNodes)
            {
                var mappedSender = _mapSender.Map((XmlElement) senderNode);
                if(mappedSender!=null)
                    result.Add(mappedSender);
            }

            return result;
        }

        public XmlElement UnMap(ObservableCollection<SenderConfigurationViewModel> input, XmlElement parrentElement)
        {
            if (_mapSender == null)
                return null;
            if (input == null)
                return null;
            if (parrentElement == null)
                return null;
            var newElement = parrentElement.CreateElement(TargetSenderConfigurationDescription.SendersCollectionElementName);
            foreach (var sender in input)
            {
                _mapSender.UnMap(sender,newElement);
            }
            return newElement;
        }
    }
}
