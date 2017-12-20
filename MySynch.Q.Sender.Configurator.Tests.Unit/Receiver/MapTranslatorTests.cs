using System.Xml;
using MySynch.Q.Common.Mappers;
using MySynch.Q.Receiver.Configurator.Configuration;
using MySynch.Q.Receiver.Configurator.Mappers;
using MySynch.Q.Receiver.Configurator.MVVM;
using MySynch.Q.Sender.Configurator.Configuration;
using MySynch.Q.Sender.Configurator.Mappers;
using MySynch.Q.Sender.Configurator.MVVM;
using NUnit.Framework;

namespace MySynch.Q.Configurators.Tests.Unit.Receiver
{
    [TestFixture]
    public class MapTranslatorTests
    {
        [Test]
        public void MapTextTranslatorNoElement()
        {
            MapTextTranslator mapTextTranslator = new MapTextTranslator();
            Assert.IsNull(mapTextTranslator.Map(null));
        }
        [Test]
        public void MapTextTranslatorElementNoAttributes()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var translatorElement = xmlDocument.CreateElement(TargetTranslatorConfigurationDescription.TranslatorElementName);
            MapTextTranslator mapTextTranslator = new MapTextTranslator();
            var translatorConfigurationViewModel = mapTextTranslator.Map(translatorElement);
            Assert.IsNotNull(translatorConfigurationViewModel);
            Assert.IsNull(translatorConfigurationViewModel.From);
            Assert.IsNull(translatorConfigurationViewModel.To);
            Assert.AreEqual(0,translatorConfigurationViewModel.Priority);
        }

        [Test]
        public void MapTextTranslatorElemenWithAttributes()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var translatorElement = xmlDocument.CreateElement(TargetTranslatorConfigurationDescription.TranslatorElementName);
            translatorElement.CreateAttribute(TargetTranslatorConfigurationDescription.From,"fromValue");
            translatorElement.CreateAttribute(TargetTranslatorConfigurationDescription.To,"toValue");
            translatorElement.CreateAttribute(TargetTranslatorConfigurationDescription.Priority, "2");
            MapTextTranslator mapTextTranslator = new MapTextTranslator();
            var translatorConfigurationViewModel = mapTextTranslator.Map(translatorElement);
            Assert.IsNotNull(translatorConfigurationViewModel);
            Assert.AreEqual("fromValue",translatorConfigurationViewModel.From);
            Assert.AreEqual("toValue",translatorConfigurationViewModel.To);
            Assert.AreEqual(2, translatorConfigurationViewModel.Priority);
        }

        [Test]
        public void MapTextTranslatorElemenWithAttributesPriortyWrong()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var translatorElement = xmlDocument.CreateElement(TargetTranslatorConfigurationDescription.TranslatorElementName);
            translatorElement.CreateAttribute(TargetTranslatorConfigurationDescription.From, "fromValue");
            translatorElement.CreateAttribute(TargetTranslatorConfigurationDescription.To, "toValue");
            translatorElement.CreateAttribute(TargetTranslatorConfigurationDescription.Priority, "xas");
            MapTextTranslator mapTextTranslator = new MapTextTranslator();
            var translatorConfigurationViewModel = mapTextTranslator.Map(translatorElement);
            Assert.IsNotNull(translatorConfigurationViewModel);
            Assert.AreEqual("fromValue", translatorConfigurationViewModel.From);
            Assert.AreEqual("toValue", translatorConfigurationViewModel.To);
            Assert.AreEqual(0, translatorConfigurationViewModel.Priority);
        }
        [Test]
        public void UnMapEmptyViewModel()
        {
            var mapTextTranslator= new MapTextTranslator();

            Assert.IsNull(mapTextTranslator.UnMap(null,null));
        }

        [Test]
        public void UnMapEmptyParrentElement()
        {
            var mapTextTranslator = new MapTextTranslator();

            Assert.IsNull(mapTextTranslator.UnMap(new TranslatorConfigurationViewModel(), null));
        }

        [Test]
        public void UnMapWithoutAValue()
        {
            var mapTextTranslator = new MapTextTranslator();
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement parrentElement = xmlDocument.CreateElement(TargetTranslatorConfigurationDescription.TranslatorsCollectionElementName);

            XmlElement translatorElement = mapTextTranslator.UnMap(new TranslatorConfigurationViewModel {From="fromValue"}, parrentElement);
            Assert.IsNotNull(translatorElement);
            Assert.AreEqual("fromValue",translatorElement.GetAttribute(TargetTranslatorConfigurationDescription.From));
            Assert.IsEmpty(translatorElement.GetAttribute(TargetTranslatorConfigurationDescription.To));
            Assert.AreEqual(parrentElement,translatorElement.ParentNode);
        }
        [Test]
        public void UnMapWithoutAKey()
        {
            var mapTextTranslator = new MapTextTranslator();
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement parrentElement = xmlDocument.CreateElement(TargetTranslatorConfigurationDescription.TranslatorsCollectionElementName);

            XmlElement translatorElement = mapTextTranslator.UnMap(new TranslatorConfigurationViewModel(), parrentElement);
            Assert.IsNull(translatorElement);
        }
        [Test]
        public void UnMapOk()
        {
            var mapTextTranslator = new MapTextTranslator();
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement parrentElement = xmlDocument.CreateElement(TargetTranslatorConfigurationDescription.TranslatorsCollectionElementName);

            XmlElement translatorElement = mapTextTranslator.UnMap(new TranslatorConfigurationViewModel { From = "fromValue" ,To="toValue", Priority=3}, parrentElement);
            Assert.IsNotNull(translatorElement);
            Assert.AreEqual("fromValue", translatorElement.GetAttribute(TargetTranslatorConfigurationDescription.From));
            Assert.AreEqual("toValue",translatorElement.GetAttribute(TargetTranslatorConfigurationDescription.To));
            Assert.AreEqual("3", translatorElement.GetAttribute(TargetTranslatorConfigurationDescription.Priority));
            Assert.AreEqual(parrentElement, translatorElement.ParentNode);
        }

    }
}
