using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MySynch.Q.Common.Contracts;
using MySynch.Q.Common.Mappers;
using MySynch.Q.Controls.MVVM;
using MySynch.Q.Sender.Configurator.Configuration;
using MySynch.Q.Sender.Configurator.MVVM;

namespace MySynch.Q.Sender.Configurator.Mappers
{
    public class MapSender:IMap<XmlElement,SenderConfigurationViewModel>
    {
        private readonly IMap<XmlElement, ObservableCollection<FilterConfigurationViewModel>> _mapFilters;
        private readonly IMap<XmlElement, ObservableCollection<QueueConfigurationViewModel>> _mapQueues;

        public MapSender(IMap<XmlElement, ObservableCollection<FilterConfigurationViewModel>> mapFilters,
            IMap<XmlElement, ObservableCollection<QueueConfigurationViewModel>> mapQueues)
        {
            _mapFilters = mapFilters;
            _mapQueues = mapQueues;
        }

        public SenderConfigurationViewModel Map(XmlElement input)
        {
            if (_mapFilters == null)
                return null;
            if (_mapQueues == null)
                return null;
            if (input == null)
                return null;

            var senderConfigurationViewModel = new SenderConfigurationViewModel();
            var localRootFolderAttributeValue = input.GetAttribute(TargetSenderConfigurationDescription.LocalRootFolder);
            var messageBodyTypeAttributeValue = input.GetAttribute(TargetSenderConfigurationDescription.MessageBodyType);
            var minMemoryAttributeValue = input.GetAttribute(TargetSenderConfigurationDescription.MinMemory);
            if (string.IsNullOrEmpty(localRootFolderAttributeValue) || string.IsNullOrEmpty(messageBodyTypeAttributeValue) ||
                string.IsNullOrEmpty(minMemoryAttributeValue))
            {
                TryAddFiltersAndQueues(senderConfigurationViewModel, input);
                return senderConfigurationViewModel;
            }
            BodyType bodyType;
            if(!Enum.TryParse(messageBodyTypeAttributeValue,out bodyType))
                bodyType=BodyType.None;

            int minMemory = 0;
            if (!int.TryParse(minMemoryAttributeValue, out minMemory))
                minMemory = 0;

            senderConfigurationViewModel.LocalRootFolderViewModel = new FolderPickerViewModel
            {
                Folder = localRootFolderAttributeValue
            };
            senderConfigurationViewModel.MessageBodyType = bodyType;

            senderConfigurationViewModel.MinMemory = minMemory;
            TryAddFiltersAndQueues(senderConfigurationViewModel,input);
            return senderConfigurationViewModel;
        }

        private void TryAddFiltersAndQueues(SenderConfigurationViewModel senderConfigurationViewModel, XmlElement senderElement)
        {
            if (senderElement.GetElementsByTagName(TargetFilterConfigurationDescription.FiltersCollectionElementName).Count != 0)
            {
                senderConfigurationViewModel.FiltersViewModel = new FiltersConfiguratorViewModel
                {
                    Filters =
                        _mapFilters.Map(
                            (XmlElement)
                            senderElement.GetElementsByTagName(
                                TargetFilterConfigurationDescription.FiltersCollectionElementName)[0])
                };
            }
            if (senderElement.GetElementsByTagName(TargetQueueConfigurationDescription.QueuesCollectionElementName).Count != 0)
            {
                senderConfigurationViewModel.QueuesViewModel = new QueuesConfiguratorViewModel
                {
                    Queues =
                        _mapQueues.Map(
                            (XmlElement)
                            senderElement.GetElementsByTagName(
                                TargetQueueConfigurationDescription.QueuesCollectionElementName)[0])
                };
            }

        }

        public XmlElement UnMap(SenderConfigurationViewModel input, XmlElement parrentElement)
        {
            if (_mapFilters == null)
                return null;
            if (_mapQueues == null)
                return null;
            if (string.IsNullOrEmpty(input?.LocalRootFolderViewModel?.Folder))
                return null;
            if (parrentElement == null)
                return null;
            var newElement = parrentElement.CreateElement(TargetSenderConfigurationDescription.SenderElementName);
            newElement.CreateAttribute(TargetSenderConfigurationDescription.LocalRootFolder,input.LocalRootFolderViewModel.Folder);
            newElement.CreateAttribute(TargetSenderConfigurationDescription.MessageBodyType,input.MessageBodyType.ToString());
            newElement.CreateAttribute(TargetSenderConfigurationDescription.MinMemory, input.MinMemory.ToString());
            if(input.FiltersViewModel!=null)
                _mapFilters.UnMap(input.FiltersViewModel.Filters, newElement);
            if(input.QueuesViewModel!=null)
                _mapQueues.UnMap(input.QueuesViewModel.Queues, newElement);
            return newElement;
        }
    }
}
