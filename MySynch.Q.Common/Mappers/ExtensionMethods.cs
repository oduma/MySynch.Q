using System.Xml;

namespace MySynch.Q.Common.Mappers
{
    public static class ExtensionMethods
    {
        public static void CreateAttribute(this XmlElement parrentElement, string attributeName, string attributeValue)
        {
            if(parrentElement?.OwnerDocument != null)
            {
                var attribute = parrentElement.OwnerDocument.CreateAttribute(attributeName);
                attribute.Value = attributeValue;
                parrentElement.Attributes.Append(attribute);

            }
        }

        public static XmlElement CreateElement(this XmlElement parrentElement, string elementName)
        {
            if (parrentElement?.OwnerDocument != null)
            {
                var newElement = (XmlElement)parrentElement.OwnerDocument.CreateNode(XmlNodeType.Element, elementName, "");
                parrentElement.AppendChild(newElement);
                return newElement;

            }
            return null;
        }
    }
}
