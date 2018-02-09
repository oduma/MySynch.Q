using System.Collections.ObjectModel;
using System.Xml;
using MySynch.Q.Common.Mappers;
using MySynch.Q.Controls.MVVM;
using MySynch.Q.Receiver.Configurator.Configuration;
using MySynch.Q.Receiver.Configurator.MVVM;

namespace MySynch.Q.Receiver.Configurator.Mappers
{
    public class MapReceiver : IMap<XmlElement, ReceiverConfigurationViewModel>
    {
        private readonly IMap<XmlElement, ObservableCollection<PostProcessorConfigurationViewModel>> _mapPostProcessors;

        public MapReceiver(IMap<XmlElement, ObservableCollection<PostProcessorConfigurationViewModel>> mapPostProcessors)
        {
            _mapPostProcessors = mapPostProcessors;
        }

        public ReceiverConfigurationViewModel Map(XmlElement input)
        {
            if (_mapPostProcessors == null)
                return null;

            if (input == null)
                return null;

            var receiverConfigurationViewModel = new ReceiverConfigurationViewModel();
            var localRootFolderAttributeValue = input.GetAttribute(TargetReceiverConfigurationDescription.LocalRootFolder);
            var nameAttributeValue = input.GetAttribute(TargetReceiverConfigurationDescription.Name);
            var queueNameAttributeValue = input.GetAttribute(TargetReceiverConfigurationDescription.QueueName);
            var hostAttributeValue = input.GetAttribute(TargetReceiverConfigurationDescription.HostName);
            var userAttributeValue = input.GetAttribute(TargetReceiverConfigurationDescription.UserName);
            var passwordAttributeValue = input.GetAttribute(TargetReceiverConfigurationDescription.Password);
            if (string.IsNullOrEmpty(localRootFolderAttributeValue)
                || string.IsNullOrEmpty(nameAttributeValue)
                || string.IsNullOrEmpty(queueNameAttributeValue)
                || string.IsNullOrEmpty(hostAttributeValue)
                || string.IsNullOrEmpty(userAttributeValue)
                || string.IsNullOrEmpty(passwordAttributeValue))
            {
                TryAddPostProcessors(receiverConfigurationViewModel, input);
                return receiverConfigurationViewModel;
            }
            receiverConfigurationViewModel.LocalRootFolderViewModel = new FolderPickerViewModel
            {
                Folder = localRootFolderAttributeValue
            };

            receiverConfigurationViewModel.Host = hostAttributeValue;
            receiverConfigurationViewModel.Name = nameAttributeValue;
            receiverConfigurationViewModel.Password = passwordAttributeValue;
            receiverConfigurationViewModel.QueueName = queueNameAttributeValue;
            receiverConfigurationViewModel.User = userAttributeValue;
            TryAddPostProcessors(receiverConfigurationViewModel, input);
            return receiverConfigurationViewModel;
        }

        private void TryAddPostProcessors(ReceiverConfigurationViewModel receiverConfigurationViewModel, XmlElement receiverElement)
        {
            if (receiverElement.GetElementsByTagName(TargetPostProcessorConfigurationDescription.PostProcessorsCollectionElementName).Count != 0)
            {
                receiverConfigurationViewModel.PostProcessorsViewModel = new PostProcessorsConfigurationViewModel
                {
                    PostProcessors =
                        _mapPostProcessors.Map(
                            (XmlElement)
                            receiverElement.GetElementsByTagName(
                                TargetPostProcessorConfigurationDescription.PostProcessorsCollectionElementName)[0])
                };
            }
        }


        public XmlElement UnMap(ReceiverConfigurationViewModel input, XmlElement parrentElement)
        {
            if (_mapPostProcessors == null)
                return null;
            if (string.IsNullOrEmpty(input?.LocalRootFolderViewModel?.Folder))
                return null;
            if (parrentElement == null)
                return null;
            var newElement = parrentElement.CreateElement(TargetReceiverConfigurationDescription.ReceiverElementName);
            newElement.CreateAttribute(TargetReceiverConfigurationDescription.LocalRootFolder, input.LocalRootFolderViewModel.Folder);
            newElement.CreateAttribute(TargetReceiverConfigurationDescription.HostName, input.Host);
            newElement.CreateAttribute(TargetReceiverConfigurationDescription.Name, input.Name);
            newElement.CreateAttribute(TargetReceiverConfigurationDescription.Password, input.Password);
            newElement.CreateAttribute(TargetReceiverConfigurationDescription.QueueName, input.QueueName);
            newElement.CreateAttribute(TargetReceiverConfigurationDescription.UserName, input.User);
            if (input.PostProcessorsViewModel != null)
                _mapPostProcessors.UnMap(input.PostProcessorsViewModel.PostProcessors, newElement);

            return newElement;
        }
    }
}