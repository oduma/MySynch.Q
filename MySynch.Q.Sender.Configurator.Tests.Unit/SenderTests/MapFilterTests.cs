using System.Xml;
using MySynch.Q.Common.Mappers;
using MySynch.Q.Sender.Configurator.Configuration;
using MySynch.Q.Sender.Configurator.Mappers;
using MySynch.Q.Sender.Configurator.MVVM;
using NUnit.Framework;

namespace MySynch.Q.Configurators.Tests.Unit.Sender
{
    [TestFixture]
    public class MapFilterTests
    {
        [Test]
        public void MapFilterNoElement()
        {
            MapFilter mapFilter = new MapFilter();
            Assert.IsNull(mapFilter.Map(null));
        }
        [Test]
        public void MapFilterElementNoAttributes()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var filterElement = xmlDocument.CreateElement(TargetFilterConfigurationDescription.FilterElementName);
            MapFilter mapFilter = new MapFilter();
            var filter = mapFilter.Map(filterElement);
            Assert.IsNotNull(filter);
            Assert.IsNull(filter.Key);
            Assert.IsNull(filter.Value);
        }

        [Test]
        public void MapFilterElemenWithAttributes()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var filterElement = xmlDocument.CreateElement(TargetFilterConfigurationDescription.FilterElementName);
            filterElement.CreateAttribute(TargetFilterConfigurationDescription.Key,"keyValue");
            filterElement.CreateAttribute(TargetFilterConfigurationDescription.Value,"valueValue");
            MapFilter mapFilter = new MapFilter();
            var filter = mapFilter.Map(filterElement);
            Assert.IsNotNull(filter);
            Assert.AreEqual("keyValue",filter.Key);
            Assert.AreEqual("valueValue",filter.Value);
        }

        [Test]
        public void UnMapEmptyViewModel()
        {
            var mapFilter= new MapFilter();

            Assert.IsNull(mapFilter.UnMap(null,null));
        }

        [Test]
        public void UnMapEmptyParrentElement()
        {
            var mapFilter = new MapFilter();

            Assert.IsNull(mapFilter.UnMap(new FilterConfigurationViewModel(), null));
        }

        [Test]
        public void UnMapWithoutAValue()
        {
            var mapFilter = new MapFilter();
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement extensionsElement = xmlDocument.CreateElement(TargetFilterConfigurationDescription.FiltersCollectionElementName);

            XmlElement filterElement = mapFilter.UnMap(new FilterConfigurationViewModel {Key="keyValue"}, extensionsElement);
            Assert.IsNotNull(filterElement);
            Assert.AreEqual("keyValue",filterElement.GetAttribute(TargetFilterConfigurationDescription.Key));
            Assert.IsEmpty(filterElement.GetAttribute(TargetFilterConfigurationDescription.Value));
            Assert.AreEqual(extensionsElement,filterElement.ParentNode);
        }
        [Test]
        public void UnMapWithoutAKey()
        {
            var mapFilter = new MapFilter();
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement extensionsElement = xmlDocument.CreateElement(TargetFilterConfigurationDescription.FiltersCollectionElementName);

            XmlElement filterElement = mapFilter.UnMap(new FilterConfigurationViewModel(), extensionsElement);
            Assert.IsNull(filterElement);
        }
        [Test]
        public void UnMapOk()
        {
            var mapFilter = new MapFilter();
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement extensionsElement = xmlDocument.CreateElement(TargetFilterConfigurationDescription.FiltersCollectionElementName);

            XmlElement filterElement = mapFilter.UnMap(new FilterConfigurationViewModel { Key = "keyValue" ,Value="valueValue"}, extensionsElement);
            Assert.IsNotNull(filterElement);
            Assert.AreEqual("keyValue", filterElement.GetAttribute(TargetFilterConfigurationDescription.Key));
            Assert.AreEqual("valueValue",filterElement.GetAttribute(TargetFilterConfigurationDescription.Value));
            Assert.AreEqual(extensionsElement, filterElement.ParentNode);
        }

    }
}
