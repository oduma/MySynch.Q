using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MySynch.Q.Common.Mappers;
using MySynch.Q.Receiver.Configurator.Configuration;
using MySynch.Q.Receiver.Configurator.MVVM;

namespace MySynch.Q.Receiver.Configurator.Mappers
{
    public class MapPostProcessor : IMap<XmlElement, PostProcessorConfigurationViewModel>
    {

        public PostProcessorConfigurationViewModel Map(XmlElement input)
        {
            if (input == null)
                return null;
            var nameAttributeValue = input.GetAttribute(TargetPostProcessorConfigurationDescription.Name);
            var valueAttributeValue = input.GetAttribute(TargetPostProcessorConfigurationDescription.Value);
            if (string.IsNullOrEmpty(nameAttributeValue) || string.IsNullOrEmpty(valueAttributeValue))
                return new PostProcessorConfigurationViewModel();
            return new PostProcessorConfigurationViewModel { Name = nameAttributeValue, Value = valueAttributeValue };
        }

        public XmlElement UnMap(PostProcessorConfigurationViewModel input, XmlElement parrentElement)
        {
            if (string.IsNullOrEmpty(input?.Name))
                return null;
            if (parrentElement == null)
                return null;
            var newElement = parrentElement.CreateElement(TargetPostProcessorConfigurationDescription.PostProcessorElementName);
            newElement.CreateAttribute(TargetPostProcessorConfigurationDescription.Name, input.Name);
            newElement.CreateAttribute(TargetPostProcessorConfigurationDescription.Value, input.Value);
            return newElement;
        }
    }
}
