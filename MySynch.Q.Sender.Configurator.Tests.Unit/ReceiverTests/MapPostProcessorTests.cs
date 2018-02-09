using System.Xml;
using MySynch.Q.Common.Mappers;
using MySynch.Q.Receiver.Configurator.Configuration;
using MySynch.Q.Receiver.Configurator.Mappers;
using MySynch.Q.Receiver.Configurator.MVVM;
using NUnit.Framework;

namespace MySynch.Q.Configurators.Tests.Unit.ReceiverTests
{
    [TestFixture]
    public class MapPostProcessorTests
    {
        [Test]
        public void MapPostProcessorNoElement()
        {
            MapPostProcessor mapPostProcessor = new MapPostProcessor();
            Assert.IsNull(mapPostProcessor.Map(null));
        }
        [Test]
        public void MapPostProcessorElementNoAttributes()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var postProcessorElement = xmlDocument.CreateElement(TargetPostProcessorConfigurationDescription.PostProcessorElementName);
            MapPostProcessor mapPostProcessor = new MapPostProcessor();
            var postProcessor = mapPostProcessor.Map(postProcessorElement);
            Assert.IsNotNull(postProcessor);
            Assert.IsNull(postProcessor.Name);
            Assert.IsNull(postProcessor.Value);
        }

        [Test]
        public void MapPostProcessorElemenWithAttributes()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var postProcessorElement = xmlDocument.CreateElement(TargetPostProcessorConfigurationDescription.PostProcessorElementName);
            postProcessorElement.CreateAttribute(TargetPostProcessorConfigurationDescription.Name, "nameValue");
            postProcessorElement.CreateAttribute(TargetPostProcessorConfigurationDescription.Value, "valueValue");
            MapPostProcessor mapPostProcessor = new MapPostProcessor();
            var postProcessor = mapPostProcessor.Map(postProcessorElement);
            Assert.IsNotNull(postProcessor);
            Assert.AreEqual("nameValue", postProcessor.Name);
            Assert.AreEqual("valueValue", postProcessor.Value);
        }

        [Test]
        public void UnMapEmptyViewModel()
        {
            var mapPostProcessor = new MapPostProcessor();

            Assert.IsNull(mapPostProcessor.UnMap(null, null));
        }

        [Test]
        public void UnMapEmptyParrentElement()
        {
            var mapPostProcessor = new MapPostProcessor();

            Assert.IsNull(mapPostProcessor.UnMap(new PostProcessorConfigurationViewModel(), null));
        }

        [Test]
        public void UnMapWithoutAValue()
        {
            var mapPostProcessor = new MapPostProcessor();
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement extensionsElement = xmlDocument.CreateElement(TargetPostProcessorConfigurationDescription.PostProcessorsCollectionElementName);

            XmlElement postProcessorElement = mapPostProcessor.UnMap(new PostProcessorConfigurationViewModel { Name = "nameValue" }, extensionsElement);
            Assert.IsNotNull(postProcessorElement);
            Assert.AreEqual("nameValue", postProcessorElement.GetAttribute(TargetPostProcessorConfigurationDescription.Name));
            Assert.IsEmpty(postProcessorElement.GetAttribute(TargetPostProcessorConfigurationDescription.Value));
            Assert.AreEqual(extensionsElement, postProcessorElement.ParentNode);
        }
        [Test]
        public void UnMapWithoutAKey()
        {
            var mapPostProcessor = new MapPostProcessor();
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement extensionsElement = xmlDocument.CreateElement(TargetPostProcessorConfigurationDescription.PostProcessorsCollectionElementName);

            XmlElement postProcessorElement = mapPostProcessor.UnMap(new PostProcessorConfigurationViewModel(), extensionsElement);
            Assert.IsNull(postProcessorElement);
        }
        [Test]
        public void UnMapOk()
        {
            var mapPostProcessor = new MapPostProcessor();
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement extensionsElement = xmlDocument.CreateElement(TargetPostProcessorConfigurationDescription.PostProcessorsCollectionElementName);

            XmlElement postProcessorElement = mapPostProcessor.UnMap(new PostProcessorConfigurationViewModel { Name = "nameValue", Value = "valueValue" }, extensionsElement);
            Assert.IsNotNull(postProcessorElement);
            Assert.AreEqual("nameValue", postProcessorElement.GetAttribute(TargetPostProcessorConfigurationDescription.Name));
            Assert.AreEqual("valueValue", postProcessorElement.GetAttribute(TargetPostProcessorConfigurationDescription.Value));
            Assert.AreEqual(extensionsElement, postProcessorElement.ParentNode);
        }

    }
}
