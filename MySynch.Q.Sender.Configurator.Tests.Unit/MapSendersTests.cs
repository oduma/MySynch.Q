using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MySynch.Q.Common.Contracts;
using MySynch.Q.Sender.Configurator.Configuration;
using MySynch.Q.Sender.Configurator.Mappers;
using MySynch.Q.Sender.Configurator.MVVM;
using NUnit.Framework;
using Rhino.Mocks;

namespace MySynch.Q.Sender.Configurator.Tests.Unit
{
    [TestFixture]
    public class MapSendersTests
    {
        [Test]
        public void MapSendersNoMapSender()
        {

            var mapSenders = new MapSenders(null);
            Assert.IsNull(mapSenders.Map(null));
        }

        [Test]
        public void MapSendersNoElement()
        {
            var mockMapSender = MockRepository.Mock<IMap<XmlElement, SenderConfigurationViewModel>>();
            var mapSenders = new MapSenders(mockMapSender);
            Assert.IsNull(mapSenders.Map(null));
        }

        [Test]
        public void MapSendersElementNoChildNodes()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var sendersElement = xmlDocument.CreateElement(TargetSenderConfigurationDescription.SendersCollectionElementName);
            var mockMapSender = MockRepository.Mock<IMap<XmlElement, SenderConfigurationViewModel>>();
            var mapSenders = new MapSenders(mockMapSender);
            var senders = mapSenders.Map(sendersElement);
            Assert.IsNotNull(senders);
            Assert.IsEmpty(senders);
        }

        [Test]
        public void MapSendersMapSenderMapsToNull()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var sendersElement = xmlDocument.CreateElement(TargetSenderConfigurationDescription.SendersCollectionElementName);
            var senderElement = sendersElement.CreateElement(TargetSenderConfigurationDescription.SenderElementName);
            var mockMapSender = MockRepository.Mock<IMap<XmlElement, SenderConfigurationViewModel>>();
            mockMapSender.Expect(m => m.Map(senderElement)).IgnoreArguments().Return(null);
            var mapSenders = new MapSenders(mockMapSender);
            var senders = mapSenders.Map(sendersElement);
            Assert.IsNotNull(senders);
            Assert.IsEmpty(senders);
        }

        [Test]
        public void MapSendersElementOk()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var sendersElement = xmlDocument.CreateElement(TargetSenderConfigurationDescription.SendersCollectionElementName);
            var senderElement = sendersElement.CreateElement(TargetSenderConfigurationDescription.SenderElementName);
            var mockMapSender = MockRepository.Mock<IMap<XmlElement, SenderConfigurationViewModel>>();
            mockMapSender.Expect(m => m.Map(senderElement))
                .IgnoreArguments()
                .Return(new SenderConfigurationViewModel
                {
                    LocalRootFolderViewModel = new RootFolderViewModel {LocalRootFolder = "localRootFolderValue"},
                    MessageBodyType = BodyType.Text,
                    MinMemory = 5
                });
            var mapSenders = new MapSenders(mockMapSender);
            var senders = mapSenders.Map(sendersElement);
            Assert.IsNotNull(senders);
            Assert.IsNotEmpty(senders);
            Assert.AreEqual(1, senders.Count);
            Assert.AreEqual("localRootFolderValue", senders[0].LocalRootFolderViewModel.LocalRootFolder);
            Assert.AreEqual(BodyType.Text, senders[0].MessageBodyType);
            Assert.AreEqual(5,senders[0].MinMemory);
        }

        [Test]
        public void UnMapSendersNoMapSender()
        {

            var mapSenders = new MapSenders(null);
            Assert.IsNull(mapSenders.UnMap(null, null));
        }

        [Test]
        public void UnMapEmptyViewModel()
        {
            var mockMapSender = MockRepository.Mock<IMap<XmlElement, SenderConfigurationViewModel>>();
            var mapSenders = new MapSenders(mockMapSender);
            Assert.IsNull(mapSenders.UnMap(null, null));
        }

        [Test]
        public void UnMapEmptyParrentElement()
        {
            var mockMapSender = MockRepository.Mock<IMap<XmlElement, SenderConfigurationViewModel>>();
            var mapSenders = new MapSenders(mockMapSender);
            Assert.IsNull(mapSenders.UnMap(new ObservableCollection<SenderConfigurationViewModel>(), null));
        }

        [Test]
        public void UnMapUnMapSenderReturnsNull()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var parrentElement = xmlDocument.CreateElement(TargetConfigurationDescription.SenderSectionElementName);
            var mockMapSender = MockRepository.Mock<IMap<XmlElement, SenderConfigurationViewModel>>();
            mockMapSender.Expect(m => m.UnMap(new SenderConfigurationViewModel(), parrentElement)).IgnoreArguments().Return(null);
            var mapSenders = new MapSenders(mockMapSender);
            var sendersElement =
                mapSenders.UnMap(
                    new ObservableCollection<SenderConfigurationViewModel>
                    {
                        new SenderConfigurationViewModel
                        {
                            LocalRootFolderViewModel = new RootFolderViewModel
                            {
                                LocalRootFolder="localRootFolderValue"
                            },
                            MessageBodyType = BodyType.Binary,
                            MinMemory =6
                        }
                    },
                    parrentElement);
            Assert.AreEqual(TargetConfigurationDescription.SenderSectionElementName, parrentElement.Name);
            Assert.AreEqual(1, parrentElement.ChildNodes.Count);
            Assert.AreEqual(TargetSenderConfigurationDescription.SendersCollectionElementName, parrentElement.ChildNodes[0].Name);
            Assert.False(parrentElement.ChildNodes[0].HasChildNodes);
            Assert.AreEqual(TargetSenderConfigurationDescription.SendersCollectionElementName, sendersElement.Name);
            Assert.AreEqual(parrentElement, sendersElement.ParentNode);
        }

        [Test]
        public void UnMapNoItemsInTheCollection()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var parrentElement = xmlDocument.CreateElement(TargetConfigurationDescription.SenderSectionElementName);
            var mockMapSender = MockRepository.Mock<IMap<XmlElement, SenderConfigurationViewModel>>();
            mockMapSender.Expect(m => m.UnMap(new SenderConfigurationViewModel(), parrentElement)).IgnoreArguments().Return(null);
            var mapSenders = new MapSenders(mockMapSender);
            var sendersElement =
                mapSenders.UnMap( new ObservableCollection<SenderConfigurationViewModel>(),
                    parrentElement);
            Assert.AreEqual(TargetConfigurationDescription.SenderSectionElementName, parrentElement.Name);
            Assert.AreEqual(1, parrentElement.ChildNodes.Count);
            Assert.AreEqual(TargetSenderConfigurationDescription.SendersCollectionElementName, parrentElement.ChildNodes[0].Name);
            Assert.False(parrentElement.ChildNodes[0].HasChildNodes);
            Assert.AreEqual(TargetSenderConfigurationDescription.SendersCollectionElementName, sendersElement.Name);
            Assert.AreEqual(parrentElement, sendersElement.ParentNode);
        }

        [Test]
        public void UnMapOk()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var parrentElement = xmlDocument.CreateElement(TargetConfigurationDescription.SenderSectionElementName);
            var mapSenders =
                new MapSenders(new MapSender(new MapFilters(new MapFilter()), new MapQueues(new MapQueue())));
            var sendersElement =
                mapSenders.UnMap(
                    new ObservableCollection<SenderConfigurationViewModel>
                    {
                        new SenderConfigurationViewModel
                        {
                            LocalRootFolderViewModel = new RootFolderViewModel
                            {
                                LocalRootFolder="localRootFolderValue"
                            },
                            MessageBodyType = BodyType.Binary,
                            MinMemory =6,
                            FiltersViewModel = new FiltersConfiguratorViewModel
                            {
                                Filters = new ObservableCollection<FilterConfigurationViewModel>
                                {
                                    new FilterConfigurationViewModel
                                    {
                                        Key    = "filterKeyValue",
                                        Value="filterValueValue"
                                    }
                                }
                            },
                            QueuesViewModel = new QueuesConfiguratorViewModel
                            {
                                Queues= new ObservableCollection<QueueConfigurationViewModel>
                                {
                                    new QueueConfigurationViewModel
                                    {
                                        Host = "queueHostValue",
                                        Name="queueNameValue",
                                        QueueName = "queueQueueNameValue",
                                        Password = "queuePasswordNameValue",
                                        User = "queueUserValue"
                                    }
                                }
                            }
                        }
                    },
                    parrentElement);
            Assert.AreEqual(parrentElement, sendersElement.ParentNode);
            Assert.False(sendersElement.HasAttributes);
            Assert.AreEqual(TargetSenderConfigurationDescription.SendersCollectionElementName, sendersElement.Name);
            Assert.AreEqual(1, sendersElement.ChildNodes.Count);
            Assert.AreEqual("localRootFolderValue", sendersElement.ChildNodes[0].Attributes[TargetSenderConfigurationDescription.LocalRootFolder].Value);
            Assert.AreEqual("Binary", sendersElement.ChildNodes[0].Attributes[TargetSenderConfigurationDescription.MessageBodyType].Value);
            Assert.AreEqual("6", sendersElement.ChildNodes[0].Attributes[TargetSenderConfigurationDescription.MinMemory].Value);
            Assert.AreEqual(2,sendersElement.ChildNodes[0].ChildNodes.Count);
            Assert.True(sendersElement.ChildNodes[0].ChildNodes.Cast<XmlElement>().Any(e=>e.Name==TargetFilterConfigurationDescription.FiltersCollectionElementName));
            Assert.True(sendersElement.ChildNodes[0].ChildNodes.Cast<XmlElement>().Any(e => e.Name == TargetQueueConfigurationDescription.QueuesCollectionElementName));
        }

    }
}
