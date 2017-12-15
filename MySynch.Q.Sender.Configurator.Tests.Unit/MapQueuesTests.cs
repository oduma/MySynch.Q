using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MySynch.Q.Sender.Configurator.Configuration;
using MySynch.Q.Sender.Configurator.Mappers;
using MySynch.Q.Sender.Configurator.MVVM;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Rhino.Mocks;

namespace MySynch.Q.Sender.Configurator.Tests.Unit
{
    [TestFixture]
    public class MapQueuesTests
    {
        [Test]
        public void MapQueuesNoMapQueue()
        {

            MapQueues mapQueues = new MapQueues(null);
            Assert.IsNull(mapQueues.Map(null));
        }

        [Test]
        public void MapQueuesNoElement()
        {
            var mockMapQueue = MockRepository.Mock<IMap<XmlElement, QueueConfigurationViewModel>>();
            MapQueues mapQueues = new MapQueues(mockMapQueue);
            Assert.IsNull(mapQueues.Map(null));
        }

        [Test]
        public void MapQueuesElementNoChildNodes()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var queueElement = xmlDocument.CreateElement(TargetQueueConfigurationDescription.QueuesCollectionElementName);
            var mockMapQueue = MockRepository.Mock<IMap<XmlElement, QueueConfigurationViewModel>>();
            MapQueues mapQueues = new MapQueues(mockMapQueue);
            var queues = mapQueues.Map(queueElement);
            Assert.IsNotNull(queues);
            Assert.IsNotNull(queues.Queues);
            Assert.IsEmpty(queues.Queues);
        }

        [Test]
        public void MapQueuesElemenOk()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var queuElement = xmlDocument.CreateElement(TargetQueueConfigurationDescription.QueuesCollectionElementName);
            queuElement.CreateElement(TargetQueueConfigurationDescription.QueueElementName);
            var mockMapQueue = MockRepository.Mock<IMap<XmlElement, QueueConfigurationViewModel>>();
            mockMapQueue.Expect(m => m.Map(queuElement))
                .IgnoreArguments()
                .Return(new QueueConfigurationViewModel
                {
                    Name = "nameValue",
                    QueueName = "queueNameValue",
                    Host = "hostValue",
                    User = "userValue",
                    Password = "passwordValue"
                });
            var mapQueues = new MapQueues(mockMapQueue);
            var queues = mapQueues.Map(queuElement);
            Assert.IsNotNull(queues);
            Assert.IsNotNull(queues.Queues);
            Assert.IsNotEmpty(queues.Queues);
            Assert.AreEqual(1, queues.Queues.Count);
            Assert.AreEqual("nameValue", queues.Queues[0].Name);
            Assert.AreEqual("queueNameValue", queues.Queues[0].QueueName);
            Assert.AreEqual("hostValue", queues.Queues[0].Host);
            Assert.AreEqual("userValue", queues.Queues[0].User);
            Assert.AreEqual("passwordValue", queues.Queues[0].Password);
        }

        
        [Test]
        public void UnMapQueuesNoMapQueue()
        {

            var mapQueues = new MapQueues(null);
            Assert.IsNull(mapQueues.UnMap(null, null));
        }

        [Test]
        public void UnMapEmptyViewModel()
        {
            var mockMapQueue = MockRepository.Mock<IMap<XmlElement, QueueConfigurationViewModel>>();
            var mapQueues = new MapQueues(mockMapQueue);
            Assert.IsNull(mapQueues.UnMap(null, null));
        }

        [Test]
        public void UnMapEmptyParrentElement()
        {
            var mockMapQueue = MockRepository.Mock<IMap<XmlElement, QueueConfigurationViewModel>>();
            var mapQueues = new MapQueues(mockMapQueue);
            Assert.IsNull(mapQueues.UnMap(new QueuesConfiguratorViewModel(), null));
        }

        [Test]
        public void UnMapUnMapQueueReturnsNull()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var parrentElement = xmlDocument.CreateElement(TargetSenderConfigurationDescription.SenderElementName);
            var mockMapQueue = MockRepository.Mock<IMap<XmlElement, QueueConfigurationViewModel>>();
            mockMapQueue.Expect(m => m.UnMap(new QueueConfigurationViewModel(), parrentElement)).IgnoreArguments().Return(null);
            var mapQueues = new MapQueues(mockMapQueue);
            var queuesElement =
                mapQueues.UnMap(
                    new QueuesConfiguratorViewModel
                    {
                        Queues =
                            new ObservableCollection<QueueConfigurationViewModel>
                            {
                                new QueueConfigurationViewModel {Name = "nameValue", QueueName = "queueNameValue",Host="hostValue", Password = "passwordValue", User="userValue"}
                            }
                    },
                    parrentElement);
            Assert.AreEqual(TargetSenderConfigurationDescription.SenderElementName, parrentElement.Name);
            Assert.AreEqual(1, parrentElement.ChildNodes.Count);
            Assert.AreEqual(TargetQueueConfigurationDescription.QueuesCollectionElementName, parrentElement.ChildNodes[0].Name);
            Assert.False(parrentElement.ChildNodes[0].HasChildNodes);
            Assert.AreEqual(TargetQueueConfigurationDescription.QueuesCollectionElementName, queuesElement.Name);
            Assert.AreEqual(parrentElement, queuesElement.ParentNode);
        }

        [Test]
        public void UnMapNoItemsInTheCollection()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var senderElement = xmlDocument.CreateElement(TargetSenderConfigurationDescription.SenderElementName);
            var mockMapQueue = MockRepository.Mock<IMap<XmlElement, QueueConfigurationViewModel>>();
            mockMapQueue.Expect(m => m.UnMap(new QueueConfigurationViewModel(), senderElement)).IgnoreArguments().Return(null);
            MapQueues mapQueues = new MapQueues(mockMapQueue);
            var queuesElement =
                mapQueues.UnMap(
                    new QueuesConfiguratorViewModel
                    {
                        Queues = new ObservableCollection<QueueConfigurationViewModel>()
                    },
                    senderElement);
            Assert.AreEqual(TargetSenderConfigurationDescription.SenderElementName, senderElement.Name);
            Assert.AreEqual(1, senderElement.ChildNodes.Count);
            Assert.AreEqual(TargetQueueConfigurationDescription.QueuesCollectionElementName, senderElement.ChildNodes[0].Name);
            Assert.False(senderElement.ChildNodes[0].HasChildNodes);
            Assert.AreEqual(TargetQueueConfigurationDescription.QueuesCollectionElementName, queuesElement.Name);
            Assert.AreEqual(senderElement, queuesElement.ParentNode);
        }
        [Test]
        public void UnMapOk()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var senderElement = xmlDocument.CreateElement(TargetSenderConfigurationDescription.SenderElementName);
            var mapQueues = new MapQueues(new MapQueue());
            var queuesElement =
                mapQueues.UnMap(
                    new QueuesConfiguratorViewModel
                    {
                        Queues =
                            new ObservableCollection<QueueConfigurationViewModel>
                            {
                                new QueueConfigurationViewModel {Name = "nameValue", QueueName = "queueNameValue", Host="hostValue", Password = "passwordValue", User = "userValue"}
                            }
                    },
                    senderElement);
            Assert.AreEqual(senderElement, queuesElement.ParentNode);
            Assert.False(queuesElement.HasAttributes);
            Assert.AreEqual(TargetQueueConfigurationDescription.QueuesCollectionElementName, queuesElement.Name);
            Assert.AreEqual(1, queuesElement.ChildNodes.Count);
            Assert.AreEqual("nameValue", queuesElement.ChildNodes[0].Attributes[TargetQueueConfigurationDescription.Name].Value);
            Assert.AreEqual("queueNameValue", queuesElement.ChildNodes[0].Attributes[TargetQueueConfigurationDescription.QueueName].Value);
            Assert.AreEqual("hostValue", queuesElement.ChildNodes[0].Attributes[TargetQueueConfigurationDescription.HostName].Value);
            Assert.AreEqual("passwordValue", queuesElement.ChildNodes[0].Attributes[TargetQueueConfigurationDescription.Password].Value);
            Assert.AreEqual("userValue", queuesElement.ChildNodes[0].Attributes[TargetQueueConfigurationDescription.UserName].Value);
        }

    }
}
