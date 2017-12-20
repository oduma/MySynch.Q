using System.Xml;
using MySynch.Q.Common.Mappers;
using MySynch.Q.Receiver.Configurator.Configuration;
using MySynch.Q.Receiver.Configurator.Mappers;
using MySynch.Q.Receiver.Configurator.MVVM;
using NUnit.Framework;

namespace MySynch.Q.Configurators.Tests.Unit.Receiver
{
    [TestFixture]
    public class MapReceiverTests
    {
        [Test]
        public void MapReceiverNoElement()
        {
            MapReceiver mapReceiver = new MapReceiver();
            Assert.IsNull(mapReceiver.Map(null));
        }
        [Test]
        public void MapReceiverElementNoAttributes()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var receiverElement = xmlDocument.CreateElement(TargetReceiverConfigurationDescription.ReceiverElementName);
            MapReceiver mapReceiver = new MapReceiver();
            var receiverConfigurationViewModel = mapReceiver.Map(receiverElement);
            Assert.IsNotNull(receiverConfigurationViewModel);
            Assert.IsNull(receiverConfigurationViewModel.Host);
            Assert.IsNull(receiverConfigurationViewModel.Name);
            Assert.IsNull(receiverConfigurationViewModel.QueueName);
            Assert.IsNull(receiverConfigurationViewModel.User);
            Assert.IsNull(receiverConfigurationViewModel.Password);
            Assert.IsNull(receiverConfigurationViewModel.LocalRootFolderViewModel);
        }

        [Test]
        public void MapReceiverElemenWithAttributes()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var receiverElement = xmlDocument.CreateElement(TargetReceiverConfigurationDescription.ReceiverElementName);
            receiverElement.CreateAttribute(TargetReceiverConfigurationDescription.HostName, "hostValue");
            receiverElement.CreateAttribute(TargetReceiverConfigurationDescription.Name, "nameValue");
            receiverElement.CreateAttribute(TargetReceiverConfigurationDescription.QueueName, "queueNameValue");
            receiverElement.CreateAttribute(TargetReceiverConfigurationDescription.Password, "passwordValue");
            receiverElement.CreateAttribute(TargetReceiverConfigurationDescription.UserName, "userValue");
            receiverElement.CreateAttribute(TargetReceiverConfigurationDescription.LocalRootFolder,"localRootFolderValue");
            MapReceiver mapReceiver = new MapReceiver();
            var receiverConfigurationViewModel = mapReceiver.Map(receiverElement);
            Assert.IsNotNull(receiverConfigurationViewModel);
            Assert.AreEqual("hostValue", receiverConfigurationViewModel.Host);
            Assert.AreEqual("nameValue", receiverConfigurationViewModel.Name);
            Assert.AreEqual("queueNameValue", receiverConfigurationViewModel.QueueName);
            Assert.AreEqual("passwordValue", receiverConfigurationViewModel.Password);
            Assert.AreEqual("userValue", receiverConfigurationViewModel.User);
            Assert.IsNotNull(receiverConfigurationViewModel.LocalRootFolderViewModel);
            Assert.AreEqual("localRootFolderValue", receiverConfigurationViewModel.LocalRootFolderViewModel.LocalRootFolder);
        }

        [Test]
        public void UnMapEmptyViewModel()
        {
            var mapReceiver = new MapReceiver();

            Assert.IsNull(mapReceiver.UnMap(null, null));
        }

        [Test]
        public void UnMapEmptyParrentElement()
        {
            var mapReceiver = new MapReceiver();

            Assert.IsNull(mapReceiver.UnMap(new ReceiverConfigurationViewModel(), null));
        }

        [Test]
        public void UnMapWithoutSomeNonKeyValues()
        {
            var mapReceiver = new MapReceiver();
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement parrentElement = xmlDocument.CreateElement(TargetReceiverConfigurationDescription.ReceiversCollectionElementName);

            XmlElement receiverElement = mapReceiver.UnMap(new ReceiverConfigurationViewModel { LocalRootFolderViewModel = new RootFolderViewModel {LocalRootFolder = "localRootFolderValue"} }, parrentElement);
            Assert.IsNotNull(receiverElement);
            Assert.AreEqual("localRootFolderValue", receiverElement.GetAttribute(TargetReceiverConfigurationDescription.LocalRootFolder));
            Assert.IsEmpty(receiverElement.GetAttribute(TargetReceiverConfigurationDescription.QueueName));
            Assert.AreEqual(parrentElement, receiverElement.ParentNode);
        }
        [Test]
        public void UnMapWithoutAKey()
        {
            var mapReceiver = new MapReceiver();
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement parrentElement = xmlDocument.CreateElement(TargetReceiverConfigurationDescription.ReceiversCollectionElementName);

            XmlElement receiverElement = mapReceiver.UnMap(new ReceiverConfigurationViewModel(), parrentElement);
            Assert.IsNull(receiverElement);
        }
        [Test]
        public void UnMapOk()
        {
            var mapReceiver = new MapReceiver();
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement parrentElement = xmlDocument.CreateElement(TargetReceiverConfigurationDescription.ReceiversCollectionElementName);

            XmlElement receiverElement =
                mapReceiver.UnMap(
                    new ReceiverConfigurationViewModel
                    {
                        LocalRootFolderViewModel = new RootFolderViewModel {LocalRootFolder = "localRootFolderValue"},
                        Host = "hostValue",
                        QueueName = "queueNameValue",
                        Name = "nameValue",
                        Password = "passwordValue",
                        User = "userValue"
                    }, parrentElement);
            Assert.IsNotNull(receiverElement);
            Assert.AreEqual("hostValue", receiverElement.GetAttribute(TargetReceiverConfigurationDescription.HostName));
            Assert.AreEqual("queueNameValue", receiverElement.GetAttribute(TargetReceiverConfigurationDescription.QueueName));
            Assert.AreEqual("nameValue", receiverElement.GetAttribute(TargetReceiverConfigurationDescription.Name));
            Assert.AreEqual("passwordValue", receiverElement.GetAttribute(TargetReceiverConfigurationDescription.Password));
            Assert.AreEqual("userValue", receiverElement.GetAttribute(TargetReceiverConfigurationDescription.UserName));
            Assert.AreEqual("localRootFolderValue", receiverElement.GetAttribute(TargetReceiverConfigurationDescription.LocalRootFolder));
            Assert.AreEqual(parrentElement, receiverElement.ParentNode);
        }

    }
}
