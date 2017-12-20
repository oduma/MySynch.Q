using System.Xml;
using MySynch.Q.Common.Mappers;
using MySynch.Q.Sender.Configurator.Configuration;
using MySynch.Q.Sender.Configurator.Mappers;
using MySynch.Q.Sender.Configurator.MVVM;
using NUnit.Framework;

namespace MySynch.Q.Configurators.Tests.Unit.Sender
{
    [TestFixture]
    public class MapQueueTests
    {
        [Test]
        public void MapQueueNoElement()
        {
            MapQueue mapQueue = new MapQueue();
            Assert.IsNull(mapQueue.Map(null));
        }
        [Test]
        public void MapQueueElementNoAttributes()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var queueElement = xmlDocument.CreateElement(TargetQueueConfigurationDescription.QueueElementName);
            MapQueue mapQueue = new MapQueue();
            var queueConfigurationViewModel = mapQueue.Map(queueElement);
            Assert.IsNotNull(queueConfigurationViewModel);
            Assert.IsNull(queueConfigurationViewModel.Host);
            Assert.IsNull(queueConfigurationViewModel.Name);
            Assert.IsNull(queueConfigurationViewModel.QueueName);
            Assert.IsNull(queueConfigurationViewModel.User);
            Assert.IsNull(queueConfigurationViewModel.Password);
        }

        [Test]
        public void MapQueueElemenWithAttributes()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var queueElement = xmlDocument.CreateElement(TargetQueueConfigurationDescription.QueueElementName);
            queueElement.CreateAttribute(TargetQueueConfigurationDescription.HostName, "hostValue");
            queueElement.CreateAttribute(TargetQueueConfigurationDescription.Name, "nameValue");
            queueElement.CreateAttribute(TargetQueueConfigurationDescription.QueueName, "queueNameValue");
            queueElement.CreateAttribute(TargetQueueConfigurationDescription.Password, "passwordValue");
            queueElement.CreateAttribute(TargetQueueConfigurationDescription.UserName, "userValue");
            MapQueue mapQueue = new MapQueue();
            var queueConfigurationViewModel = mapQueue.Map(queueElement);
            Assert.IsNotNull(queueConfigurationViewModel);
            Assert.AreEqual("hostValue", queueConfigurationViewModel.Host);
            Assert.AreEqual("nameValue", queueConfigurationViewModel.Name);
            Assert.AreEqual("queueNameValue", queueConfigurationViewModel.QueueName);
            Assert.AreEqual("passwordValue", queueConfigurationViewModel.Password);
            Assert.AreEqual("userValue", queueConfigurationViewModel.User);
        }

        [Test]
        public void UnMapEmptyViewModel()
        {
            var mapQueue = new MapQueue();

            Assert.IsNull(mapQueue.UnMap(null, null));
        }

        [Test]
        public void UnMapEmptyParrentElement()
        {
            var mapQueue = new MapQueue();

            Assert.IsNull(mapQueue.UnMap(new QueueConfigurationViewModel(), null));
        }

        [Test]
        public void UnMapWithoutSomeNonKeyValues()
        {
            var mapQueue = new MapQueue();
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement queuesElement = xmlDocument.CreateElement(TargetQueueConfigurationDescription.QueuesCollectionElementName);

            XmlElement queueElement = mapQueue.UnMap(new QueueConfigurationViewModel { Name = "nameValue" }, queuesElement);
            Assert.IsNotNull(queueElement);
            Assert.AreEqual("nameValue", queueElement.GetAttribute(TargetQueueConfigurationDescription.Name));
            Assert.IsEmpty(queueElement.GetAttribute(TargetQueueConfigurationDescription.QueueElementName));
            Assert.AreEqual(queuesElement, queueElement.ParentNode);
        }
        [Test]
        public void UnMapWithoutAKey()
        {
            var mapQueue = new MapQueue();
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement queuesElement = xmlDocument.CreateElement(TargetQueueConfigurationDescription.QueuesCollectionElementName);

            XmlElement queuElement = mapQueue.UnMap(new QueueConfigurationViewModel(), queuesElement);
            Assert.IsNull(queuElement);
        }
        [Test]
        public void UnMapOk()
        {
            var mapQueue = new MapQueue();
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement queuesElement = xmlDocument.CreateElement(TargetQueueConfigurationDescription.QueuesCollectionElementName);

            XmlElement queueElement = mapQueue.UnMap(new QueueConfigurationViewModel { Host = "hostValue", QueueName = "queueNameValue",Name="nameValue", Password="passwordValue", User="userValue" }, queuesElement);
            Assert.IsNotNull(queueElement);
            Assert.AreEqual("hostValue", queueElement.GetAttribute(TargetQueueConfigurationDescription.HostName));
            Assert.AreEqual("queueNameValue", queueElement.GetAttribute(TargetQueueConfigurationDescription.QueueName));
            Assert.AreEqual("nameValue", queueElement.GetAttribute(TargetQueueConfigurationDescription.Name));
            Assert.AreEqual("passwordValue", queueElement.GetAttribute(TargetQueueConfigurationDescription.Password));
            Assert.AreEqual("userValue", queueElement.GetAttribute(TargetQueueConfigurationDescription.UserName));
            Assert.AreEqual(queuesElement, queueElement.ParentNode);
        }

    }
}
