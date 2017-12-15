using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MySynch.Q.Sender.Configurator.Configuration;
using MySynch.Q.Sender.Configurator.MVVM;

namespace MySynch.Q.Sender.Configurator.Mappers
{
    public class MapFilter:IMap<XmlElement,FilterConfigurationViewModel>
    {

        public FilterConfigurationViewModel Map(XmlElement input)
        {
            if (input == null)
                return null;
            var keyAttributeValue = input.GetAttribute(TargetFilterConfigurationDescription.Key);
            var valueAttributeValue = input.GetAttribute(TargetFilterConfigurationDescription.Value);
            if(string.IsNullOrEmpty(keyAttributeValue) || string.IsNullOrEmpty(valueAttributeValue))
                return new FilterConfigurationViewModel();
            return new FilterConfigurationViewModel { Key = keyAttributeValue, Value = valueAttributeValue };
        }

        public XmlElement UnMap(FilterConfigurationViewModel input, XmlElement parrentElement)
        {
            if (string.IsNullOrEmpty(input?.Key))
                return null;
            if (parrentElement == null)
                return null;
            var newElement = parrentElement.CreateElement(TargetFilterConfigurationDescription.FilterElementName);
            newElement.CreateAttribute(TargetFilterConfigurationDescription.Key,input.Key);
            newElement.CreateAttribute(TargetFilterConfigurationDescription.Value,input.Value);
            return newElement;
        }
    }
}
