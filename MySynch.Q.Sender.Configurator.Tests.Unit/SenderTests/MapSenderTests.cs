using System.Collections.ObjectModel;
using System.Xml;
using MySynch.Q.Common.Contracts;
using MySynch.Q.Common.Mappers;
using MySynch.Q.Controls.MVVM;
using MySynch.Q.Sender.Configurator.Configuration;
using MySynch.Q.Sender.Configurator.Mappers;
using MySynch.Q.Sender.Configurator.MVVM;
using NUnit.Framework;
using Rhino.Mocks;

namespace MySynch.Q.Configurators.Tests.Unit.Sender
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
            MapSender mapSender = new MapSender(new MapCollectionNodeNoAttributes<FilterConfigurationViewModel>(null, TargetFilterConfigurationDescription.FiltersCollectionElementName), null);
            Assert.IsNull(mapSender.Map(senderElement));
        }

        [Test]
        public void MapSenderElementNoAttributesAndNoSubElements()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var senderElement = xmlDocument.CreateElement(TargetSenderConfigurationDescription.SenderElementName);
            var mapSender = new MapSender(new MapCollectionNodeNoAttributes<FilterConfigurationViewModel>(null,TargetFilterConfigurationDescription.FiltersCollectionElementName), new MapCollectionNodeNoAttributes<QueueConfigurationViewModel>(null,TargetQueueConfigurationDescription.QueuesCollectionElementName));
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
            senderElement.CreateElement(TargetFilterConfigurationDescription.FiltersCollectionElementName);
            senderElement.CreateElement(TargetQueueConfigurationDescription.QueuesCollectionElementName);
            var mockMapFilter = MockRepository.Mock<IMap<XmlElement, FilterConfigurationViewModel>>();
            var mockMapQueue = MockRepository.Mock<IMap<XmlElement, QueueConfigurationViewModel>>();

            var mapSender =
                new MapSender(
                    new MapCollectionNodeNoAttributes<FilterConfigurationViewModel>(mockMapFilter,
                        TargetFilterConfigurationDescription.FiltersCollectionElementName),
                    new MapCollectionNodeNoAttributes<QueueConfigurationViewModel>(mockMapQueue,
                        TargetQueueConfigurationDescription.QueuesCollectionElementName));
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
            senderElement.CreateElement(TargetFilterConfigurationDescription.FiltersCollectionElementName);
            senderElement.CreateElement(TargetQueueConfigurationDescription.QueuesCollectionElementName);
            var mockMapFilter = MockRepository.Mock<IMap<XmlElement, FilterConfigurationViewModel>>();
            var mockMapQueue = MockRepository.Mock<IMap<XmlElement, QueueConfigurationViewModel>>();

            var mapSender = new MapSender(new MapCollectionNodeNoAttributes<FilterConfigurationViewModel>(mockMapFilter,TargetFilterConfigurationDescription.FiltersCollectionElementName), new MapCollectionNodeNoAttributes<QueueConfigurationViewModel>(mockMapQueue,TargetQueueConfigurationDescription.QueuesCollectionElementName));
            var senderConfigurationViewModel = mapSender.Map(senderElement);
            Assert.IsNotNull(senderConfigurationViewModel);
            Assert.AreEqual("localRootFolderValue", senderConfigurationViewModel.LocalRootFolderViewModel.Folder);
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
            senderElement.CreateElement(TargetFilterConfigurationDescription.FiltersCollectionElementName);
            senderElement.CreateElement(TargetQueueConfigurationDescription.QueuesCollectionElementName);
            var mockMapFilter = MockRepository.Mock<IMap<XmlElement, FilterConfigurationViewModel>>();
            var mockMapQueue = MockRepository.Mock<IMap<XmlElement, QueueConfigurationViewModel>>();

            var mapSender = new MapSender(new MapCollectionNodeNoAttributes<FilterConfigurationViewModel>(mockMapFilter,TargetFilterConfigurationDescription.FiltersCollectionElementName), new MapCollectionNodeNoAttributes<QueueConfigurationViewModel>(mockMapQueue,TargetQueueConfigurationDescription.QueuesCollectionElementName));
            var senderConfigurationViewModel = mapSender.Map(senderElement);
            Assert.IsNotNull(senderConfigurationViewModel);
            Assert.AreEqual("localRootFolderValue", senderConfigurationViewModel.LocalRootFolderViewModel.Folder);
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
            senderElement.CreateElement(TargetFilterConfigurationDescription.FiltersCollectionElementName);
            senderElement.CreateElement(TargetQueueConfigurationDescription.QueuesCollectionElementName);
            var mockMapFilter = MockRepository.Mock<IMap<XmlElement, FilterConfigurationViewModel>>();
            var mockMapQueue = MockRepository.Mock<IMap<XmlElement, QueueConfigurationViewModel>>();

            var mapSender = new MapSender(new MapCollectionNodeNoAttributes<FilterConfigurationViewModel>(mockMapFilter,TargetFilterConfigurationDescription.FiltersCollectionElementName), new MapCollectionNodeNoAttributes<QueueConfigurationViewModel>(mockMapQueue,TargetQueueConfigurationDescription.QueuesCollectionElementName));
            var senderConfigurationViewModel = mapSender.Map(senderElement);
            Assert.IsNotNull(senderConfigurationViewModel);
            Assert.AreEqual("localRootFolderValue", senderConfigurationViewModel.LocalRootFolderViewModel.Folder);
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
            senderElement.CreateElement(TargetQueueConfigurationDescription.QueuesCollectionElementName);
            var mockMapFilters = MockRepository.Mock<IMap<XmlElement, ObservableCollection<FilterConfigurationViewModel>>>();
            mockMapFilters.Expect(f => f.Map(filtersElement)).Return(null);
            var mockMapQueue = MockRepository.Mock<IMap<XmlElement, QueueConfigurationViewModel>>();

            var mapSender = new MapSender(mockMapFilters, new MapCollectionNodeNoAttributes<QueueConfigurationViewModel>(mockMapQueue,TargetQueueConfigurationDescription.QueuesCollectionElementName));
            var senderConfigurationViewModel = mapSender.Map(senderElement);
            Assert.IsNotNull(senderConfigurationViewModel);
            Assert.AreEqual("localRootFolderValue", senderConfigurationViewModel.LocalRootFolderViewModel.Folder);
            Assert.AreEqual(BodyType.Text, senderConfigurationViewModel.MessageBodyType);
            Assert.AreEqual(2, senderConfigurationViewModel.MinMemory);
            Assert.IsNull(senderConfigurationViewModel.FiltersViewModel.Filters);
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
            senderElement.CreateElement(TargetFilterConfigurationDescription.FiltersCollectionElementName);
            var queuesElement = senderElement.CreateElement(TargetQueueConfigurationDescription.QueuesCollectionElementName);
            var mockMapFilter = MockRepository.Mock<IMap<XmlElement, FilterConfigurationViewModel>>();
            var mockMapQueues = MockRepository.Mock<IMap<XmlElement, ObservableCollection<QueueConfigurationViewModel>>>();
            mockMapQueues.Expect(q => q.Map(queuesElement)).Return(null);
            var mapSender = new MapSender(new MapCollectionNodeNoAttributes<FilterConfigurationViewModel>(mockMapFilter,TargetFilterConfigurationDescription.FiltersCollectionElementName), mockMapQueues);
            var senderConfigurationViewModel = mapSender.Map(senderElement);
            Assert.IsNotNull(senderConfigurationViewModel);
            Assert.AreEqual("localRootFolderValue", senderConfigurationViewModel.LocalRootFolderViewModel.Folder);
            Assert.AreEqual(BodyType.Text, senderConfigurationViewModel.MessageBodyType);
            Assert.AreEqual(2, senderConfigurationViewModel.MinMemory);
            Assert.IsNotNull(senderConfigurationViewModel.FiltersViewModel);
            Assert.IsNull(senderConfigurationViewModel.QueuesViewModel.Queues);
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
            var mapSender = new MapSender(new MapCollectionNodeNoAttributes<FilterConfigurationViewModel>(null,TargetFilterConfigurationDescription.FiltersCollectionElementName), null);

            Assert.IsNull(mapSender.UnMap(null, null));
        }

        [Test]
        public void UnMapEmptyViewModel()
        {
            var mapSender = new MapSender(new MapCollectionNodeNoAttributes<FilterConfigurationViewModel>(null,TargetFilterConfigurationDescription.FiltersCollectionElementName), new MapCollectionNodeNoAttributes<QueueConfigurationViewModel>(null,TargetQueueConfigurationDescription.QueuesCollectionElementName));

            Assert.IsNull(mapSender.UnMap(null, null));
        }

        [Test]
        public void UnMapEmptyParrentElement()
        {
            var mapSender = new MapSender(new MapCollectionNodeNoAttributes<FilterConfigurationViewModel>(null,TargetFilterConfigurationDescription.FiltersCollectionElementName), new MapCollectionNodeNoAttributes<QueueConfigurationViewModel>(null,TargetQueueConfigurationDescription.QueuesCollectionElementName));

            Assert.IsNull(mapSender.UnMap(new SenderConfigurationViewModel(), null));
        }

        [Test]
        public void UnMapNoLocalRootFolderInput()
        {
            var mapSender = new MapSender(new MapCollectionNodeNoAttributes<FilterConfigurationViewModel>(null,TargetFilterConfigurationDescription.FiltersCollectionElementName), new MapCollectionNodeNoAttributes<QueueConfigurationViewModel>(null,TargetQueueConfigurationDescription.QueuesCollectionElementName));

            var xmlDocument = new XmlDocument();
            var parrentElement = xmlDocument.CreateElement(TargetSenderConfigurationDescription.SendersCollectionElementName);
            Assert.IsNull(mapSender.UnMap(new SenderConfigurationViewModel(), parrentElement));
        }

        [Test]
        public void UnMapWithoutSomeNonKeyValues()
        {
            var mapSender = new MapSender(new MapCollectionNodeNoAttributes<FilterConfigurationViewModel>(null,TargetFilterConfigurationDescription.FiltersCollectionElementName), new MapCollectionNodeNoAttributes<QueueConfigurationViewModel>(null,TargetQueueConfigurationDescription.QueuesCollectionElementName));
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement parrentElement = xmlDocument.CreateElement(TargetSenderConfigurationDescription.SendersCollectionElementName);

            XmlElement senderElement =
                mapSender.UnMap(
                    new SenderConfigurationViewModel
                    {
                        LocalRootFolderViewModel = new FolderPickerViewModel {Folder = "localRootFolderValue"}
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
            var mapSender = new MapSender(new MapCollectionNodeNoAttributes<FilterConfigurationViewModel>(null,TargetFilterConfigurationDescription.FiltersCollectionElementName), new MapCollectionNodeNoAttributes<QueueConfigurationViewModel>(null,TargetQueueConfigurationDescription.QueuesCollectionElementName));
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement parrentElement = xmlDocument.CreateElement(TargetSenderConfigurationDescription.SendersCollectionElementName);

            XmlElement senderElement = mapSender.UnMap(new SenderConfigurationViewModel {LocalRootFolderViewModel = new FolderPickerViewModel()}, parrentElement);
            Assert.IsNull(senderElement);
        }

        [Test]
        public void UnMapUnMapFiltersReturnsNull()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var parrentElement = xmlDocument.CreateElement(TargetSenderConfigurationDescription.SendersCollectionElementName);
            var mockMapFilters = MockRepository.Mock<IMap<XmlElement, ObservableCollection<FilterConfigurationViewModel>>>();
            mockMapFilters.Expect(
                m =>
                    m.UnMap(
                        new ObservableCollection<FilterConfigurationViewModel> {new FilterConfigurationViewModel()},
                        parrentElement)).IgnoreArguments().Return(null);
            var mapSender = new MapSender(mockMapFilters,new MapCollectionNodeNoAttributes<QueueConfigurationViewModel>(null,TargetQueueConfigurationDescription.QueuesCollectionElementName));
            var senderElement =
                mapSender.UnMap(
                    new SenderConfigurationViewModel
                    {
                        
                        LocalRootFolderViewModel=new FolderPickerViewModel {Folder="localRootFolderValue"}, 
                        FiltersViewModel = new FiltersConfigurationViewModel
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
            var mockMapQueues = MockRepository.Mock<IMap<XmlElement, ObservableCollection<QueueConfigurationViewModel>>>();
            mockMapQueues.Expect(m => m.UnMap(new ObservableCollection<QueueConfigurationViewModel> { new QueueConfigurationViewModel()}, parrentElement)).IgnoreArguments().Return(null);
            var mapSender = new MapSender(new MapCollectionNodeNoAttributes<FilterConfigurationViewModel>(null,TargetFilterConfigurationDescription.FiltersCollectionElementName), mockMapQueues);
            var senderElement =
                mapSender.UnMap(
                    new SenderConfigurationViewModel
                    {

                        LocalRootFolderViewModel = new FolderPickerViewModel { Folder = "localRootFolderValue" },
                        QueuesViewModel = new QueuesConfigurationViewModel
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

            var mockMapFilters = MockRepository.Mock<IMap<XmlElement, ObservableCollection<FilterConfigurationViewModel>>>();
            mockMapFilters.Expect(f => f.UnMap(new ObservableCollection<FilterConfigurationViewModel> {new FilterConfigurationViewModel()}, senderElement)).IgnoreArguments().Return(filtersElement);
            var mockMapQueues = MockRepository.Mock<IMap<XmlElement, ObservableCollection<QueueConfigurationViewModel>>>();
            mockMapQueues.Expect(q => q.UnMap(new ObservableCollection<QueueConfigurationViewModel> {new QueueConfigurationViewModel()},senderElement)).IgnoreArguments().Return(queuesElement);

            var mapSender = new MapSender(mockMapFilters, mockMapQueues);

            XmlElement senderResultElement = mapSender.UnMap(new SenderConfigurationViewModel
            {
                LocalRootFolderViewModel = new FolderPickerViewModel { Folder="localRootFolderValue"},
                MessageBodyType=BodyType.Text,
                MinMemory=4,
                FiltersViewModel = new FiltersConfigurationViewModel(),
                QueuesViewModel=new QueuesConfigurationViewModel()
            }, parrentElement);

            Assert.IsNotNull(senderResultElement);
            Assert.AreEqual("localRootFolderValue", senderResultElement.GetAttribute(TargetSenderConfigurationDescription.LocalRootFolder));
            Assert.AreEqual("Text", senderResultElement.GetAttribute(TargetSenderConfigurationDescription.MessageBodyType));
            Assert.AreEqual("4", senderResultElement.GetAttribute(TargetSenderConfigurationDescription.MinMemory));
            Assert.AreEqual(parrentElement, senderResultElement.ParentNode);

        }


    }
}
