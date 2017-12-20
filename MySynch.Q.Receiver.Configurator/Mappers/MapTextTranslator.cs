using System.Xml;
using MySynch.Q.Common.Mappers;
using MySynch.Q.Receiver.Configurator.Configuration;
using MySynch.Q.Receiver.Configurator.MVVM;

namespace MySynch.Q.Receiver.Configurator.Mappers
{
    public class MapTextTranslator : IMap<XmlElement, TranslatorConfigurationViewModel>
    {
        public TranslatorConfigurationViewModel Map(XmlElement input)
        {
            if (input == null)
                return null;
            var fromAttributeValue = input.GetAttribute(TargetTranslatorConfigurationDescription.From);
            var toNameAttributeValue = input.GetAttribute(TargetTranslatorConfigurationDescription.To);
            var priorityAttributeValue = input.GetAttribute(TargetTranslatorConfigurationDescription.Priority);
            if (string.IsNullOrEmpty(fromAttributeValue) || string.IsNullOrEmpty(toNameAttributeValue) || string.IsNullOrEmpty(priorityAttributeValue))
                return new TranslatorConfigurationViewModel();
            int priority = 0;
            if (!int.TryParse(priorityAttributeValue, out priority))
                priority = 0;

            return new TranslatorConfigurationViewModel
            {
                Priority = priority,
                From = fromAttributeValue,
                To = toNameAttributeValue
            };
        }

        public XmlElement UnMap(TranslatorConfigurationViewModel input, XmlElement parrentElement)
        {
            if (string.IsNullOrEmpty(input?.From))
                return null;
            if (parrentElement == null)
                return null;


            var result = parrentElement.CreateElement(TargetTranslatorConfigurationDescription.TranslatorElementName);
            result.CreateAttribute(TargetTranslatorConfigurationDescription.From, input.From);
            result.CreateAttribute(TargetTranslatorConfigurationDescription.To, input.To);
            result.CreateAttribute(TargetTranslatorConfigurationDescription.Priority, input.Priority.ToString());
            return result;
        }
    }
}