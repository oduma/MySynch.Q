using System.Xml;
using MySynch.Q.Sender.Configurator.Configuration;
using MySynch.Q.Sender.Configurator.MVVM;

namespace MySynch.Q.Sender.Configurator.Mappers
{
    public class MapQueue:IMap<XmlElement,QueueConfigurationViewModel>
    {
        public QueueConfigurationViewModel Map(XmlElement input)
        {
            if (input == null)
                return null;
            var nameAttributeValue = input.GetAttribute(TargetQueueConfigurationDescription.Name);
            var queueNameAttributeValue = input.GetAttribute(TargetQueueConfigurationDescription.QueueName);
            var hostAttributeValue = input.GetAttribute(TargetQueueConfigurationDescription.HostName);
            var userAttributeValue = input.GetAttribute(TargetQueueConfigurationDescription.UserName);
            var passwordAttributeValue = input.GetAttribute(TargetQueueConfigurationDescription.Password);
            if(string.IsNullOrEmpty(nameAttributeValue)|| string.IsNullOrEmpty(queueNameAttributeValue) || string.IsNullOrEmpty(hostAttributeValue) || string.IsNullOrEmpty(userAttributeValue) || string.IsNullOrEmpty(passwordAttributeValue))
                return new QueueConfigurationViewModel();
            return new QueueConfigurationViewModel
            {
                Host = hostAttributeValue,
                Name = nameAttributeValue,
                Password = passwordAttributeValue,
                QueueName = queueNameAttributeValue,
                User = userAttributeValue
            };
        }

        public XmlElement UnMap(QueueConfigurationViewModel input, XmlElement parrentElement)
        {
            if (string.IsNullOrEmpty(input?.Name))
                return null;
            if (parrentElement == null)
                return null;


            var result = parrentElement.CreateElement(TargetQueueConfigurationDescription.QueueElementName);
            result.CreateAttribute(TargetQueueConfigurationDescription.HostName,input.Host);
            result.CreateAttribute(TargetQueueConfigurationDescription.Name, input.Name);
            result.CreateAttribute(TargetQueueConfigurationDescription.Password, input.Password);
            result.CreateAttribute(TargetQueueConfigurationDescription.QueueName, input.QueueName);
            result.CreateAttribute(TargetQueueConfigurationDescription.UserName, input.User);
            return result;
        }

    }
}
