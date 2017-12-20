using System;
using System.Xml;
using MySynch.Q.Common.Contracts;
using MySynch.Q.Common.Mappers;
using MySynch.Q.Receiver.Configurator.Configuration;
using MySynch.Q.Receiver.Configurator.MVVM;

namespace MySynch.Q.Receiver.Configurator.Mappers
{
    public class MapReceiver : IMap<XmlElement, ReceiverConfigurationViewModel>
    {
        public ReceiverConfigurationViewModel Map(XmlElement input)
        {
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
                return new ReceiverConfigurationViewModel();
            return new ReceiverConfigurationViewModel
            {
                LocalRootFolderViewModel = new RootFolderViewModel
                {
                    LocalRootFolder = localRootFolderAttributeValue
                },

                Host = hostAttributeValue,
                Name = nameAttributeValue,
                Password = passwordAttributeValue,
                QueueName = queueNameAttributeValue,
                User = userAttributeValue
            };
        }


        public XmlElement UnMap(ReceiverConfigurationViewModel input, XmlElement parrentElement)
        {
            if (string.IsNullOrEmpty(input?.LocalRootFolderViewModel?.LocalRootFolder))
                return null;
            if (parrentElement == null)
                return null;
            var newElement = parrentElement.CreateElement(TargetReceiverConfigurationDescription.ReceiverElementName);
            newElement.CreateAttribute(TargetReceiverConfigurationDescription.LocalRootFolder, input.LocalRootFolderViewModel.LocalRootFolder);
            newElement.CreateAttribute(TargetReceiverConfigurationDescription.HostName, input.Host);
            newElement.CreateAttribute(TargetReceiverConfigurationDescription.Name, input.Name);
            newElement.CreateAttribute(TargetReceiverConfigurationDescription.Password, input.Password);
            newElement.CreateAttribute(TargetReceiverConfigurationDescription.QueueName, input.QueueName);
            newElement.CreateAttribute(TargetReceiverConfigurationDescription.UserName, input.User);
            return newElement;
        }
    }
}