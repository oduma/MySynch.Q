using System.Collections.ObjectModel;
using System.Xml;
using MySynch.Q.Common.Mappers;
using MySynch.Q.Controls.MVVM;
using MySynch.Q.Receiver.Configurator.Configuration;
using MySynch.Q.Receiver.Configurator.Mappers;
using MySynch.Q.Receiver.Configurator.MVVM;
using NUnit.Framework;
using Rhino.Mocks;

namespace MySynch.Q.Configurators.Tests.Unit.Receiver
{
    [TestFixture]
    public class MapReceiverTests
    {
        [Test]
        public void MapReceiverNoElement()
        {
            MapReceiver mapReceiver = new MapReceiver(null);
            Assert.IsNull(mapReceiver.Map(null));
        }

        [Test]
        public void MapSenderNoMapFilters()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var receiverElement = xmlDocument.CreateElement(TargetReceiverConfigurationDescription.ReceiverElementName);
            MapReceiver mapReceiver = new MapReceiver(null);
            Assert.IsNull(mapReceiver.Map(receiverElement));
        }

        [Test]
        public void MapReceiverElementNoAttributesAndNodSubElements()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var receiverElement = xmlDocument.CreateElement(TargetReceiverConfigurationDescription.ReceiverElementName);
            MapReceiver mapReceiver =
                new MapReceiver(new MapCollectionNodeNoAttributes<PostProcessorConfigurationViewModel>(null,
                    TargetPostProcessorConfigurationDescription.PostProcessorsCollectionElementName));
            var receiverConfigurationViewModel = mapReceiver.Map(receiverElement);
            Assert.IsNotNull(receiverConfigurationViewModel);
            Assert.IsNull(receiverConfigurationViewModel.Host);
            Assert.IsNull(receiverConfigurationViewModel.Name);
            Assert.IsNull(receiverConfigurationViewModel.QueueName);
            Assert.IsNull(receiverConfigurationViewModel.User);
            Assert.IsNull(receiverConfigurationViewModel.Password);
            Assert.IsNull(receiverConfigurationViewModel.LocalRootFolderViewModel);
            Assert.IsNull(receiverConfigurationViewModel.PostProcessorsViewModel);
        }

        [Test]
        public void MapReceiverElementNoAttributesAndEmptySubElements()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var receiverElement = xmlDocument.CreateElement(TargetReceiverConfigurationDescription.ReceiverElementName);
            receiverElement.CreateElement(
                TargetPostProcessorConfigurationDescription.PostProcessorsCollectionElementName);
            var mockMapPostProcessor = MockRepository.Mock<IMap<XmlElement, PostProcessorConfigurationViewModel>>();
            MapReceiver mapReceiver =
                new MapReceiver(new MapCollectionNodeNoAttributes<PostProcessorConfigurationViewModel>(mockMapPostProcessor,
                    TargetPostProcessorConfigurationDescription.PostProcessorsCollectionElementName));
            var receiverConfigurationViewModel = mapReceiver.Map(receiverElement);
            Assert.IsNotNull(receiverConfigurationViewModel);
            Assert.IsNull(receiverConfigurationViewModel.Host);
            Assert.IsNull(receiverConfigurationViewModel.Name);
            Assert.IsNull(receiverConfigurationViewModel.QueueName);
            Assert.IsNull(receiverConfigurationViewModel.User);
            Assert.IsNull(receiverConfigurationViewModel.Password);
            Assert.IsNull(receiverConfigurationViewModel.LocalRootFolderViewModel);
            Assert.IsNotNull(receiverConfigurationViewModel.PostProcessorsViewModel);
        }
        [Test]
        public void MapReceiverElemenWithAttributesAndPosProcessorsMapsNull()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var receiverElement = xmlDocument.CreateElement(TargetReceiverConfigurationDescription.ReceiverElementName);
            receiverElement.CreateAttribute(TargetReceiverConfigurationDescription.HostName, "hostValue");
            receiverElement.CreateAttribute(TargetReceiverConfigurationDescription.Name, "nameValue");
            receiverElement.CreateAttribute(TargetReceiverConfigurationDescription.QueueName, "queueNameValue");
            receiverElement.CreateAttribute(TargetReceiverConfigurationDescription.Password, "passwordValue");
            receiverElement.CreateAttribute(TargetReceiverConfigurationDescription.UserName, "userValue");
            receiverElement.CreateAttribute(TargetReceiverConfigurationDescription.LocalRootFolder,"localRootFolderValue");
            var postProcessorsElement =
                receiverElement.CreateElement(
                    TargetPostProcessorConfigurationDescription.PostProcessorsCollectionElementName);
            var mockMapPostProcessors = MockRepository.Mock<IMap<XmlElement, ObservableCollection<PostProcessorConfigurationViewModel>>>();
            mockMapPostProcessors.Expect(m => m.Map(postProcessorsElement)).Return(null);
            MapReceiver mapReceiver = new MapReceiver(mockMapPostProcessors);
            var receiverConfigurationViewModel = mapReceiver.Map(receiverElement);
            Assert.IsNotNull(receiverConfigurationViewModel);
            Assert.AreEqual("hostValue", receiverConfigurationViewModel.Host);
            Assert.AreEqual("nameValue", receiverConfigurationViewModel.Name);
            Assert.AreEqual("queueNameValue", receiverConfigurationViewModel.QueueName);
            Assert.AreEqual("passwordValue", receiverConfigurationViewModel.Password);
            Assert.AreEqual("userValue", receiverConfigurationViewModel.User);
            Assert.IsNotNull(receiverConfigurationViewModel.LocalRootFolderViewModel);
            Assert.AreEqual("localRootFolderValue", receiverConfigurationViewModel.LocalRootFolderViewModel.Folder);
            Assert.IsNull(receiverConfigurationViewModel.PostProcessorsViewModel.PostProcessors);
        }

        [Test]
        public void UnMapNoMapFilters()
        {
            var mapReceiver = new MapReceiver(null);

            Assert.IsNull(mapReceiver.UnMap(null, null));
        }

        [Test]
        public void UnMapEmptyViewModel()
        {
            var mapReceiver =
                new MapReceiver(new MapCollectionNodeNoAttributes<PostProcessorConfigurationViewModel>(null,
                    TargetPostProcessorConfigurationDescription.PostProcessorsCollectionElementName));

            Assert.IsNull(mapReceiver.UnMap(null, null));
        }

        [Test]
        public void UnMapEmptyParrentElement()
        {
            var mapReceiver =
                new MapReceiver(new MapCollectionNodeNoAttributes<PostProcessorConfigurationViewModel>(null,
                    TargetPostProcessorConfigurationDescription.PostProcessorsCollectionElementName));

            Assert.IsNull(mapReceiver.UnMap(new ReceiverConfigurationViewModel(), null));
        }

        [Test]
        public void UnMapWithoutSomeNonKeyValues()
        {
            var mapReceiver =
                new MapReceiver(new MapCollectionNodeNoAttributes<PostProcessorConfigurationViewModel>(null,
                    TargetPostProcessorConfigurationDescription.PostProcessorsCollectionElementName));
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement parrentElement = xmlDocument.CreateElement(TargetReceiverConfigurationDescription.ReceiversCollectionElementName);

            XmlElement receiverElement = mapReceiver.UnMap(new ReceiverConfigurationViewModel { LocalRootFolderViewModel = new FolderPickerViewModel {Folder = "localRootFolderValue"} }, parrentElement);
            Assert.IsNotNull(receiverElement);
            Assert.AreEqual("localRootFolderValue", receiverElement.GetAttribute(TargetReceiverConfigurationDescription.LocalRootFolder));
            Assert.IsEmpty(receiverElement.GetAttribute(TargetReceiverConfigurationDescription.QueueName));
            Assert.AreEqual(parrentElement, receiverElement.ParentNode);
        }
        [Test]
        public void UnMapWithoutAKey()
        {
            var mapReceiver =
                new MapReceiver(new MapCollectionNodeNoAttributes<PostProcessorConfigurationViewModel>(null,
                    TargetPostProcessorConfigurationDescription.PostProcessorsCollectionElementName));
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement parrentElement = xmlDocument.CreateElement(TargetReceiverConfigurationDescription.ReceiversCollectionElementName);

            XmlElement receiverElement = mapReceiver.UnMap(new ReceiverConfigurationViewModel(), parrentElement);
            Assert.IsNull(receiverElement);
        }

        [Test]
        public void UnMapUnMapFiltersReturnsNull()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var parrentElement = xmlDocument.CreateElement(TargetReceiverConfigurationDescription.ReceiversCollectionElementName);
            var mockMapPostProcessors = MockRepository.Mock<IMap<XmlElement, ObservableCollection<PostProcessorConfigurationViewModel>>>();
            mockMapPostProcessors.Expect(
                m =>
                    m.UnMap(
                        new ObservableCollection<PostProcessorConfigurationViewModel> { new PostProcessorConfigurationViewModel() },
                        parrentElement)).IgnoreArguments().Return(null);
            var mapReceiver = new MapReceiver(mockMapPostProcessors);
            var receiverElement =
                mapReceiver.UnMap(
                    new ReceiverConfigurationViewModel
                    {

                        LocalRootFolderViewModel = new FolderPickerViewModel { Folder = "localRootFolderValue" },
                        PostProcessorsViewModel = new PostProcessorsConfigurationViewModel
                        {
                            PostProcessors =
                            new ObservableCollection<PostProcessorConfigurationViewModel>
                            {
                                new PostProcessorConfigurationViewModel {Name = "keyValue", Value = "valueValue"}
                            }
                        }
                    },
                    parrentElement);
            Assert.AreEqual(TargetReceiverConfigurationDescription.ReceiversCollectionElementName, parrentElement.Name);
            Assert.AreEqual(1, parrentElement.ChildNodes.Count);
            Assert.AreEqual(TargetReceiverConfigurationDescription.ReceiverElementName, parrentElement.ChildNodes[0].Name);
            Assert.False(parrentElement.ChildNodes[0].HasChildNodes);
            Assert.AreEqual(TargetReceiverConfigurationDescription.ReceiverElementName, receiverElement.Name);
            Assert.AreEqual(parrentElement, receiverElement.ParentNode);
        }

        [Test]
        public void UnMapOk()
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement parrentElement = xmlDocument.CreateElement(TargetReceiverConfigurationDescription.ReceiversCollectionElementName);
            XmlElement receiverElement =
                parrentElement.CreateElement(TargetReceiverConfigurationDescription.ReceiverElementName);
            XmlElement postProcessorsElement =
                receiverElement.CreateElement(
                    TargetPostProcessorConfigurationDescription.PostProcessorsCollectionElementName);

            var mockMapPostProcessors = MockRepository.Mock<IMap<XmlElement, ObservableCollection<PostProcessorConfigurationViewModel>>>();
            mockMapPostProcessors.Expect(f => f.UnMap(new ObservableCollection<PostProcessorConfigurationViewModel> { new PostProcessorConfigurationViewModel() }, receiverElement)).IgnoreArguments().Return(postProcessorsElement);

            var mapReceiver = new MapReceiver(mockMapPostProcessors);
            XmlElement receiverResultElement =
                mapReceiver.UnMap(
                    new ReceiverConfigurationViewModel
                    {
                        LocalRootFolderViewModel = new FolderPickerViewModel {Folder = "localRootFolderValue"},
                        Host = "hostValue",
                        QueueName = "queueNameValue",
                        Name = "nameValue",
                        Password = "passwordValue",
                        User = "userValue",
                        PostProcessorsViewModel = new PostProcessorsConfigurationViewModel()
                    }, parrentElement);
            Assert.IsNotNull(receiverResultElement);
            Assert.AreEqual("hostValue", receiverResultElement.GetAttribute(TargetReceiverConfigurationDescription.HostName));
            Assert.AreEqual("queueNameValue", receiverResultElement.GetAttribute(TargetReceiverConfigurationDescription.QueueName));
            Assert.AreEqual("nameValue", receiverResultElement.GetAttribute(TargetReceiverConfigurationDescription.Name));
            Assert.AreEqual("passwordValue", receiverResultElement.GetAttribute(TargetReceiverConfigurationDescription.Password));
            Assert.AreEqual("userValue", receiverResultElement.GetAttribute(TargetReceiverConfigurationDescription.UserName));
            Assert.AreEqual("localRootFolderValue", receiverResultElement.GetAttribute(TargetReceiverConfigurationDescription.LocalRootFolder));
            Assert.AreEqual(parrentElement, receiverResultElement.ParentNode);
        }

    }
}
