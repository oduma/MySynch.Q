﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MySynch.Q.Common.Contracts;
using MySynch.Q.Common.Mappers;
using MySynch.Q.Sender.Configurator.Configuration;
using MySynch.Q.Sender.Configurator.Mappers;
using MySynch.Q.Sender.Configurator.MVVM;
using NUnit.Framework.Internal;
using NUnit.Framework;
using Rhino.Mocks;

namespace MySynch.Q.Sender.Configurator.Tests.Unit
{
    [TestFixture]
    public class MapSenderTests
    {
        [Test]
        public void MapSenderNoElement()
        {
            MapSender mapSender = new MapSender(null,null);
            Assert.IsNull(mapSender.Map(null));
        }
        [Test]
        public void MapSenderNoMapFilters()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var senderElement = xmlDocument.CreateElement(TargetSenderConfigurationDescription.SenderElementName);
            MapSender mapSender = new MapSender(null, null);
            Assert.IsNull(mapSender.Map(senderElement));
        }

        [Test]
        public void MapSenderNoMapQueues()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var senderElement = xmlDocument.CreateElement(TargetSenderConfigurationDescription.SenderElementName);
            MapSender mapSender = new MapSender(new MapFilters(null), null);
            Assert.IsNull(mapSender.Map(senderElement));
        }

        [Test]
        public void MapSenderElementNoAttributesAndNoSubElements()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var senderElement = xmlDocument.CreateElement(TargetSenderConfigurationDescription.SenderElementName);
            var mapSender = new MapSender(new MapFilters(null), new MapQueues(null));
            var senderConfigurationViewModel = mapSender.Map(senderElement);
            Assert.IsNotNull(senderConfigurationViewModel);
            Assert.IsNull(senderConfigurationViewModel.FiltersViewModel);
            Assert.AreEqual(BodyType.None, senderConfigurationViewModel.MessageBodyType);
            Assert.IsNull(senderConfigurationViewModel.LocalRootFolderViewModel);
            Assert.AreEqual(0,senderConfigurationViewModel.MinMemory);
            Assert.IsNull(senderConfigurationViewModel.QueuesViewModel);
        }

        [Test]
        public void MapSenderElementNoAttributesAndEmptySubElements()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var senderElement = xmlDocument.CreateElement(TargetSenderConfigurationDescription.SenderElementName);
            var filtersElement = senderElement.CreateElement(TargetFilterConfigurationDescription.FiltersCollectionElementName);
            var queuesElement = senderElement.CreateElement(TargetQueueConfigurationDescription.QueuesCollectionElementName);
            var mockMapFilter = MockRepository.Mock<IMap<XmlElement, FilterConfigurationViewModel>>();
            var mockMapQueue = MockRepository.Mock<IMap<XmlElement, QueueConfigurationViewModel>>();

            var mapSender = new MapSender(new MapFilters(mockMapFilter), new MapQueues(mockMapQueue));
            var senderConfigurationViewModel = mapSender.Map(senderElement);
            Assert.IsNotNull(senderConfigurationViewModel);
            Assert.IsNotNull(senderConfigurationViewModel.FiltersViewModel);
            Assert.AreEqual(BodyType.None, senderConfigurationViewModel.MessageBodyType);
            Assert.IsNull(senderConfigurationViewModel.LocalRootFolderViewModel);
            Assert.AreEqual(0, senderConfigurationViewModel.MinMemory);
            Assert.IsNotNull(senderConfigurationViewModel.QueuesViewModel);
        }

        [Test]
        public void MapSenderElemenWithAttributesAndEmptySubElements()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var senderElement = xmlDocument.CreateElement(TargetSenderConfigurationDescription.SenderElementName);
            senderElement.CreateAttribute(TargetSenderConfigurationDescription.LocalRootFolder, "localRootFolderValue");
            senderElement.CreateAttribute(TargetSenderConfigurationDescription.MessageBodyType, "Text");
            senderElement.CreateAttribute(TargetSenderConfigurationDescription.MinMemory, "2");
            var filtersElement = senderElement.CreateElement(TargetFilterConfigurationDescription.FiltersCollectionElementName);
            var queuesElement = senderElement.CreateElement(TargetQueueConfigurationDescription.QueuesCollectionElementName);
            var mockMapFilter = MockRepository.Mock<IMap<XmlElement, FilterConfigurationViewModel>>();
            var mockMapQueue = MockRepository.Mock<IMap<XmlElement, QueueConfigurationViewModel>>();

            var mapSender = new MapSender(new MapFilters(mockMapFilter), new MapQueues(mockMapQueue));
            var senderConfigurationViewModel = mapSender.Map(senderElement);
            Assert.IsNotNull(senderConfigurationViewModel);
            Assert.AreEqual("localRootFolderValue", senderConfigurationViewModel.LocalRootFolderViewModel.LocalRootFolder);
            Assert.AreEqual(BodyType.Text, senderConfigurationViewModel.MessageBodyType);
            Assert.AreEqual(2, senderConfigurationViewModel.MinMemory);
            Assert.IsNotNull(senderConfigurationViewModel.FiltersViewModel);
            Assert.IsNotNull(senderConfigurationViewModel.QueuesViewModel);
        }

        [Test]
        public void MapSenderElemenWithAttributesWrongMessageBodyTypeAndEmptySubElements()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var senderElement = xmlDocument.CreateElement(TargetSenderConfigurationDescription.SenderElementName);
            senderElement.CreateAttribute(TargetSenderConfigurationDescription.LocalRootFolder, "localRootFolderValue");
            senderElement.CreateAttribute(TargetSenderConfigurationDescription.MessageBodyType, "Textxxx");
            senderElement.CreateAttribute(TargetSenderConfigurationDescription.MinMemory, "2");
            var filtersElement = senderElement.CreateElement(TargetFilterConfigurationDescription.FiltersCollectionElementName);
            var queuesElement = senderElement.CreateElement(TargetQueueConfigurationDescription.QueuesCollectionElementName);
            var mockMapFilter = MockRepository.Mock<IMap<XmlElement, FilterConfigurationViewModel>>();
            var mockMapQueue = MockRepository.Mock<IMap<XmlElement, QueueConfigurationViewModel>>();

            var mapSender = new MapSender(new MapFilters(mockMapFilter), new MapQueues(mockMapQueue));
            var senderConfigurationViewModel = mapSender.Map(senderElement);
            Assert.IsNotNull(senderConfigurationViewModel);
            Assert.AreEqual("localRootFolderValue", senderConfigurationViewModel.LocalRootFolderViewModel.LocalRootFolder);
            Assert.AreEqual(BodyType.None, senderConfigurationViewModel.MessageBodyType);
            Assert.AreEqual(2, senderConfigurationViewModel.MinMemory);
            Assert.IsNotNull(senderConfigurationViewModel.FiltersViewModel);
            Assert.IsNotNull(senderConfigurationViewModel.QueuesViewModel);
        }

        [Test]
        public void MapSenderElemenWithAttributesWrongMinMemoryAndEmptySubElements()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var senderElement = xmlDocument.CreateElement(TargetSenderConfigurationDescription.SenderElementName);
            senderElement.CreateAttribute(TargetSenderConfigurationDescription.LocalRootFolder, "localRootFolderValue");
            senderElement.CreateAttribute(TargetSenderConfigurationDescription.MessageBodyType, "Binary");
            senderElement.CreateAttribute(TargetSenderConfigurationDescription.MinMemory, "xas");
            var filtersElement = senderElement.CreateElement(TargetFilterConfigurationDescription.FiltersCollectionElementName);
            var queuesElement = senderElement.CreateElement(TargetQueueConfigurationDescription.QueuesCollectionElementName);
            var mockMapFilter = MockRepository.Mock<IMap<XmlElement, FilterConfigurationViewModel>>();
            var mockMapQueue = MockRepository.Mock<IMap<XmlElement, QueueConfigurationViewModel>>();

            var mapSender = new MapSender(new MapFilters(mockMapFilter), new MapQueues(mockMapQueue));
            var senderConfigurationViewModel = mapSender.Map(senderElement);
            Assert.IsNotNull(senderConfigurationViewModel);
            Assert.AreEqual("localRootFolderValue", senderConfigurationViewModel.LocalRootFolderViewModel.LocalRootFolder);
            Assert.AreEqual(BodyType.Binary, senderConfigurationViewModel.MessageBodyType);
            Assert.AreEqual(0, senderConfigurationViewModel.MinMemory);
            Assert.IsNotNull(senderConfigurationViewModel.FiltersViewModel);
            Assert.IsNotNull(senderConfigurationViewModel.QueuesViewModel);
        }

        [Test]
        public void MapSenderElemenWithAttributesAndFiltersMapsNull()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var senderElement = xmlDocument.CreateElement(TargetSenderConfigurationDescription.SenderElementName);
            senderElement.CreateAttribute(TargetSenderConfigurationDescription.LocalRootFolder, "localRootFolderValue");
            senderElement.CreateAttribute(TargetSenderConfigurationDescription.MessageBodyType, "Text");
            senderElement.CreateAttribute(TargetSenderConfigurationDescription.MinMemory, "2");
            var filtersElement = senderElement.CreateElement(TargetFilterConfigurationDescription.FiltersCollectionElementName);
            var queuesElement = senderElement.CreateElement(TargetQueueConfigurationDescription.QueuesCollectionElementName);
            var mockMapFilters = MockRepository.Mock<IMap<XmlElement, FiltersConfiguratorViewModel>>();
            mockMapFilters.Expect(f => f.Map(filtersElement)).Return(null);
            var mockMapQueue = MockRepository.Mock<IMap<XmlElement, QueueConfigurationViewModel>>();

            var mapSender = new MapSender(mockMapFilters, new MapQueues(mockMapQueue));
            var senderConfigurationViewModel = mapSender.Map(senderElement);
            Assert.IsNotNull(senderConfigurationViewModel);
            Assert.AreEqual("localRootFolderValue", senderConfigurationViewModel.LocalRootFolderViewModel.LocalRootFolder);
            Assert.AreEqual(BodyType.Text, senderConfigurationViewModel.MessageBodyType);
            Assert.AreEqual(2, senderConfigurationViewModel.MinMemory);
            Assert.IsNull(senderConfigurationViewModel.FiltersViewModel);
            Assert.IsNotNull(senderConfigurationViewModel.QueuesViewModel);
        }

        [Test]
        public void MapSenderElemenWithAttributesQueuesMapsToNull()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var senderElement = xmlDocument.CreateElement(TargetSenderConfigurationDescription.SenderElementName);
            senderElement.CreateAttribute(TargetSenderConfigurationDescription.LocalRootFolder, "localRootFolderValue");
            senderElement.CreateAttribute(TargetSenderConfigurationDescription.MessageBodyType, "Text");
            senderElement.CreateAttribute(TargetSenderConfigurationDescription.MinMemory, "2");
            var filtersElement = senderElement.CreateElement(TargetFilterConfigurationDescription.FiltersCollectionElementName);
            var queuesElement = senderElement.CreateElement(TargetQueueConfigurationDescription.QueuesCollectionElementName);
            var mockMapFilter = MockRepository.Mock<IMap<XmlElement, FilterConfigurationViewModel>>();
            var mockMapQueues = MockRepository.Mock<IMap<XmlElement, QueuesConfiguratorViewModel>>();
            mockMapQueues.Expect(q => q.Map(queuesElement)).Return(null);
            var mapSender = new MapSender(new MapFilters(mockMapFilter), mockMapQueues);
            var senderConfigurationViewModel = mapSender.Map(senderElement);
            Assert.IsNotNull(senderConfigurationViewModel);
            Assert.AreEqual("localRootFolderValue", senderConfigurationViewModel.LocalRootFolderViewModel.LocalRootFolder);
            Assert.AreEqual(BodyType.Text, senderConfigurationViewModel.MessageBodyType);
            Assert.AreEqual(2, senderConfigurationViewModel.MinMemory);
            Assert.IsNotNull(senderConfigurationViewModel.FiltersViewModel);
            Assert.IsNull(senderConfigurationViewModel.QueuesViewModel);
        }

        [Test]
        public void UnMapNoMapFilters()
        {
            var mapSender = new MapSender(null,null);

            Assert.IsNull(mapSender.UnMap(null, null));
        }

        [Test]
        public void UnMapNoMapQueues()
        {
            var mapSender = new MapSender(new MapFilters(null), null);

            Assert.IsNull(mapSender.UnMap(null, null));
        }

        [Test]
        public void UnMapEmptyViewModel()
        {
            var mapSender = new MapSender(new MapFilters(null), new MapQueues(null));

            Assert.IsNull(mapSender.UnMap(null, null));
        }

        [Test]
        public void UnMapEmptyParrentElement()
        {
            var mapSender = new MapSender(new MapFilters(null), new MapQueues(null));

            Assert.IsNull(mapSender.UnMap(new SenderConfigurationViewModel(), null));
        }

        [Test]
        public void UnMapNoLocalRootFolderInput()
        {
            var mapSender = new MapSender(new MapFilters(null), new MapQueues(null));

            var xmlDocument = new XmlDocument();
            var parrentElement = xmlDocument.CreateElement(TargetSenderConfigurationDescription.SendersCollectionElementName);
            Assert.IsNull(mapSender.UnMap(new SenderConfigurationViewModel(), parrentElement));
        }

        [Test]
        public void UnMapWithoutSomeNonKeyValues()
        {
            var mapSender = new MapSender(new MapFilters(null), new MapQueues(null));
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement parrentElement = xmlDocument.CreateElement(TargetSenderConfigurationDescription.SendersCollectionElementName);

            XmlElement senderElement =
                mapSender.UnMap(
                    new SenderConfigurationViewModel
                    {
                        LocalRootFolderViewModel = new RootFolderViewModel {LocalRootFolder = "localRootFolderValue"}
                    }, parrentElement);
            Assert.IsNotNull(senderElement);
            Assert.AreEqual("localRootFolderValue", senderElement.GetAttribute(TargetSenderConfigurationDescription.LocalRootFolder));
            Assert.AreEqual("None",senderElement.GetAttribute(TargetSenderConfigurationDescription.MessageBodyType));
            Assert.AreEqual("0",senderElement.GetAttribute(TargetSenderConfigurationDescription.MinMemory));
            Assert.AreEqual(parrentElement, senderElement.ParentNode);
        }

        [Test]
        public void UnMapWithoutAKey()
        {
            var mapSender = new MapSender(new MapFilters(null), new MapQueues(null));
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement parrentElement = xmlDocument.CreateElement(TargetSenderConfigurationDescription.SendersCollectionElementName);

            XmlElement senderElement = mapSender.UnMap(new SenderConfigurationViewModel {LocalRootFolderViewModel = new RootFolderViewModel()}, parrentElement);
            Assert.IsNull(senderElement);
        }

        [Test]
        public void UnMapUnMapFiltersReturnsNull()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var parrentElement = xmlDocument.CreateElement(TargetSenderConfigurationDescription.SendersCollectionElementName);
            var mockMapFilters = MockRepository.Mock<IMap<XmlElement, FiltersConfiguratorViewModel>>();
            mockMapFilters.Expect(m => m.UnMap(new FiltersConfiguratorViewModel(), parrentElement)).IgnoreArguments().Return(null);
            var mapSender = new MapSender(mockMapFilters,new MapQueues(null));
            var senderElement =
                mapSender.UnMap(
                    new SenderConfigurationViewModel
                    {
                        
                        LocalRootFolderViewModel=new RootFolderViewModel {LocalRootFolder="localRootFolderValue"}, 
                        FiltersViewModel = new FiltersConfiguratorViewModel
                        {
                            Filters =
                            new ObservableCollection<FilterConfigurationViewModel>
                            {
                                new FilterConfigurationViewModel {Key = "keyValue", Value = "valueValue"}
                            }
                        }
                    },
                    parrentElement);
            Assert.AreEqual(TargetSenderConfigurationDescription.SendersCollectionElementName, parrentElement.Name);
            Assert.AreEqual(1, parrentElement.ChildNodes.Count);
            Assert.AreEqual(TargetSenderConfigurationDescription.SenderElementName, parrentElement.ChildNodes[0].Name);
            Assert.False(parrentElement.ChildNodes[0].HasChildNodes);
            Assert.AreEqual(TargetSenderConfigurationDescription.SenderElementName, senderElement.Name);
            Assert.AreEqual(parrentElement, senderElement.ParentNode);
        }

        [Test]
        public void UnMapUnMapQueuesReturnsNull()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var parrentElement = xmlDocument.CreateElement(TargetSenderConfigurationDescription.SendersCollectionElementName);
            var mockMapQueues = MockRepository.Mock<IMap<XmlElement, QueuesConfiguratorViewModel>>();
            mockMapQueues.Expect(m => m.UnMap(new QueuesConfiguratorViewModel(), parrentElement)).IgnoreArguments().Return(null);
            var mapSender = new MapSender(new MapFilters(null), mockMapQueues);
            var senderElement =
                mapSender.UnMap(
                    new SenderConfigurationViewModel
                    {

                        LocalRootFolderViewModel = new RootFolderViewModel { LocalRootFolder = "localRootFolderValue" },
                        QueuesViewModel = new QueuesConfiguratorViewModel
                        {
                            Queues =
                            new ObservableCollection<QueueConfigurationViewModel>
                            {
                                new QueueConfigurationViewModel {Name = "nameValue", Host = "hostValue"}
                            }
                        }
                    },
                    parrentElement);
            Assert.AreEqual(TargetSenderConfigurationDescription.SendersCollectionElementName, parrentElement.Name);
            Assert.AreEqual(1, parrentElement.ChildNodes.Count);
            Assert.AreEqual(TargetSenderConfigurationDescription.SenderElementName, parrentElement.ChildNodes[0].Name);
            Assert.False(parrentElement.ChildNodes[0].HasChildNodes);
            Assert.AreEqual(TargetSenderConfigurationDescription.SenderElementName, senderElement.Name);
            Assert.AreEqual(parrentElement, senderElement.ParentNode);
        }

        [Test]
        public void UnMapOk()
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement parrentElement = xmlDocument.CreateElement(TargetSenderConfigurationDescription.SendersCollectionElementName);
            XmlElement senderElement = parrentElement.CreateElement(TargetSenderConfigurationDescription.SenderElementName);
            XmlElement queuesElement = parrentElement.CreateElement(TargetQueueConfigurationDescription.QueuesCollectionElementName);
            XmlElement filtersElement = parrentElement.CreateElement(TargetFilterConfigurationDescription.FiltersCollectionElementName);

            var mockMapFilters = MockRepository.Mock<IMap<XmlElement, FiltersConfiguratorViewModel>>();
            mockMapFilters.Expect(f => f.UnMap(new FiltersConfiguratorViewModel(), senderElement)).IgnoreArguments().Return(filtersElement);
            var mockMapQueues = MockRepository.Mock<IMap<XmlElement, QueuesConfiguratorViewModel>>();
            mockMapQueues.Expect(q => q.UnMap(new QueuesConfiguratorViewModel(),senderElement)).IgnoreArguments().Return(queuesElement);

            var mapSender = new MapSender(mockMapFilters, mockMapQueues);

            XmlElement senderResultElement = mapSender.UnMap(new SenderConfigurationViewModel
            {
                LocalRootFolderViewModel = new RootFolderViewModel { LocalRootFolder="localRootFolderValue"},
                MessageBodyType=BodyType.Text,
                MinMemory=4,
                FiltersViewModel = new FiltersConfiguratorViewModel(),
                QueuesViewModel=new QueuesConfiguratorViewModel()
            }, parrentElement);

            Assert.IsNotNull(senderResultElement);
            Assert.AreEqual("localRootFolderValue", senderResultElement.GetAttribute(TargetSenderConfigurationDescription.LocalRootFolder));
            Assert.AreEqual("Text", senderResultElement.GetAttribute(TargetSenderConfigurationDescription.MessageBodyType));
            Assert.AreEqual("4", senderResultElement.GetAttribute(TargetSenderConfigurationDescription.MinMemory));
            Assert.AreEqual(parrentElement, senderResultElement.ParentNode);

        }


    }
}