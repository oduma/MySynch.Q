using System.Collections.ObjectModel;
using System.Xml;
using MySynch.Q.Common.Mappers;
using MySynch.Q.Sender.Configurator.Configuration;
using MySynch.Q.Sender.Configurator.MVVM;

namespace MySynch.Q.Sender.Configurator.Mappers
{
    public class MapQueues: IMap<XmlElement,QueuesConfiguratorViewModel>
    {
        private readonly IMap<XmlElement, QueueConfigurationViewModel> _mapQueue;

        public MapQueues(IMap<XmlElement,QueueConfigurationViewModel> mapQueue)
        {
            _mapQueue = mapQueue;
        }

        public QueuesConfiguratorViewModel Map(XmlElement input)
        {
            if (_mapQueue == null)
                return null;
            if (input == null)
                return null;

            var queuesConfiguratorViewModel = new QueuesConfiguratorViewModel
            {
                Queues = new ObservableCollection<QueueConfigurationViewModel>()
            };
            foreach (XmlElement queueNode in input.ChildNodes)
            {
                queuesConfiguratorViewModel.Queues.Add(_mapQueue.Map(queueNode));
            }
            return queuesConfiguratorViewModel;
        }

        public XmlElement UnMap(QueuesConfiguratorViewModel input, XmlElement parrentXmlElement)
        {
            if (_mapQueue == null)
                return null;
            if (input == null)
                return null;
            if (parrentXmlElement == null)
                return null;

            XmlElement newElement = parrentXmlElement.CreateElement(TargetQueueConfigurationDescription.QueuesCollectionElementName);
            foreach (var queue in input.Queues)
            {
                _mapQueue.UnMap(queue,newElement);
            }
            return newElement;
        }
    }
}
