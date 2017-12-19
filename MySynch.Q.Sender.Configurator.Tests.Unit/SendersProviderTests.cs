using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MySynch.Q.Common.Configurators;
using MySynch.Q.Common.Configurators.Description;
using MySynch.Q.Common.Contracts;
using MySynch.Q.Common.Mappers;
using MySynch.Q.Sender.Configurator.Configuration;
using MySynch.Q.Sender.Configurator.Mappers;
using MySynch.Q.Sender.Configurator.MVVM;
using NUnit.Framework;
using Rhino.Mocks;

namespace MySynch.Q.Sender.Configurator.Tests.Unit
{
    [TestFixture]
    public class SendersProviderTests
    {
        [Test]
        public void GetSendersNoSectionIdentifierTests()
        {
            ConfigurationToViewModelProvider<SenderConfigurationViewModel> sendersProvider = new ConfigurationToViewModelProvider<SenderConfigurationViewModel>(null);
            var sendersCollection = sendersProvider.GetViewModelsCollection(null);
            Assert.IsNotNull(sendersCollection);
            Assert.IsEmpty(sendersCollection);
        }

        [Test]
        public void GetSendersNoSenderLocatorFilePathTests()
        {
            ConfigurationToViewModelProvider<SenderConfigurationViewModel> sendersProvider = new ConfigurationToViewModelProvider<SenderConfigurationViewModel>(null);
            var sendersCollection = sendersProvider.GetViewModelsCollection(new ConfigurationSectionLocator());
            Assert.IsNotNull(sendersCollection);
            Assert.IsEmpty(sendersCollection);
        }

        [Test]
        public void GetSendersSenderLocatorFilePathNotValidTests()
        {
            ConfigurationToViewModelProvider<SenderConfigurationViewModel> sendersProvider = new ConfigurationToViewModelProvider<SenderConfigurationViewModel>(null);
            Assert.Throws<FileNotFoundException>(() => sendersProvider.GetViewModelsCollection(new ConfigurationSectionLocator {FilePath = "aaa"}));
        }

        [Test]
        public void GetSendersNoMapSendersTests()
        {
            ConfigurationToViewModelProvider<SenderConfigurationViewModel> sendersProvider = new ConfigurationToViewModelProvider<SenderConfigurationViewModel>(null);
            var sendersCollection = sendersProvider.GetViewModelsCollection(new ConfigurationSectionLocator
            {
                FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.config")
            });
            Assert.IsNotNull(sendersCollection);
            Assert.IsEmpty(sendersCollection);
        }

        [Test]
        public void GetSendersSenderLocatorEmptySectionIdentifierTests()
        {

            ConfigurationToViewModelProvider<SenderConfigurationViewModel> sendersProvider =
                new ConfigurationToViewModelProvider<SenderConfigurationViewModel>(
                    new MapSenders(new MapSender(new MapFilters(new MapFilter()), new MapQueues(new MapQueue()))));
            Assert.Throws<ConfigurationErrorsException>(
                () =>
                    sendersProvider.GetViewModelsCollection(new ConfigurationSectionLocator
                    {
                        FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.config")
                    }));
        }
        [Test]
        public void GetSendersSenderLocatorNotFoundSectionIdentifierTests()
        {

            ConfigurationToViewModelProvider<SenderConfigurationViewModel> sendersProvider =
                new ConfigurationToViewModelProvider<SenderConfigurationViewModel>(
                    new MapSenders(new MapSender(new MapFilters(new MapFilter()), new MapQueues(new MapQueue()))));
            Assert.Throws<ConfigurationErrorsException>(
                () =>
                    sendersProvider.GetViewModelsCollection(new ConfigurationSectionLocator
                    {
                        FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.config"),
                        SectionIdentifier="wrongsectionIdentifier"
                    }));
        }

        [Test]
        public void GetSendersMapSendersMapsToNullTests()
        {
            var mockMapSenders =
                MockRepository.Mock<IMap<XmlElement, ObservableCollection<SenderConfigurationViewModel>>>();
            var fakeDoc = new XmlDocument();
            var fakeElement = fakeDoc.CreateElement("fake");
            mockMapSenders.Expect(s => s.Map(fakeElement)).IgnoreArguments().Return(null);
            ConfigurationToViewModelProvider<SenderConfigurationViewModel> sendersProvider = new ConfigurationToViewModelProvider<SenderConfigurationViewModel>(mockMapSenders);
            var sendersResult = sendersProvider.GetViewModelsCollection(new ConfigurationSectionLocator
                    {
                        FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.config"),
                        SectionIdentifier = TargetConfigurationDescription.SenderSectionElementName
                    });
            Assert.IsNotNull(sendersResult);
            Assert.IsEmpty(sendersResult);
        }

        [Test]
        public void GetSendersOkTests()
        {
            ConfigurationToViewModelProvider<SenderConfigurationViewModel> sendersProvider =
                new ConfigurationToViewModelProvider<SenderConfigurationViewModel>(
                    new MapSenders(new MapSender(new MapFilters(new MapFilter()), new MapQueues(new MapQueue()))));
            var senders = sendersProvider.GetViewModelsCollection(new ConfigurationSectionLocator
            {
                FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.config"),
                SectionIdentifier = TargetConfigurationDescription.SenderSectionElementName
            });
            Assert.IsNotEmpty(senders);
            Assert.AreEqual(2, senders.Count());
            Assert.AreEqual(@"C:\Code\work\Sciendo\MySynch.Q\Source-Debug\Music\",senders[0].LocalRootFolderViewModel.LocalRootFolder);
            Assert.AreEqual(BodyType.Binary, senders[0].MessageBodyType);
            Assert.AreEqual(60000, senders[0].MinMemory);
            Assert.IsNull(senders[0].FiltersViewModel);
            Assert.IsNotEmpty(senders[0].QueuesViewModel.Queues);
            Assert.AreEqual(1,senders[0].QueuesViewModel.Queues.Count);
            Assert.AreEqual("Music",senders[0].QueuesViewModel.Queues[0].Name);
            Assert.AreEqual("music", senders[0].QueuesViewModel.Queues[0].QueueName);
            Assert.AreEqual("octo-laptop", senders[0].QueuesViewModel.Queues[0].Host);
            Assert.AreEqual("user", senders[0].QueuesViewModel.Queues[0].Password);
            Assert.AreEqual("user", senders[0].QueuesViewModel.Queues[0].User);
            Assert.AreEqual(@"C:\Code\work\Sciendo\MySynch.Q\Source-Debug\Playlists\", senders[1].LocalRootFolderViewModel.LocalRootFolder);
            Assert.AreEqual(BodyType.Text, senders[1].MessageBodyType);
            Assert.AreEqual(60000, senders[1].MinMemory);
            Assert.IsNotEmpty(senders[1].FiltersViewModel.Filters);
            Assert.AreEqual(2,senders[1].FiltersViewModel.Filters.Count);
            Assert.AreEqual("m3u",senders[1].FiltersViewModel.Filters[0].Key);
            Assert.AreEqual(".m3u", senders[1].FiltersViewModel.Filters[0].Value);
            Assert.AreEqual("xspf", senders[1].FiltersViewModel.Filters[1].Key);
            Assert.AreEqual(".xspf", senders[1].FiltersViewModel.Filters[1].Value);
            Assert.IsNotEmpty(senders[1].QueuesViewModel.Queues);
            Assert.AreEqual(1, senders[1].QueuesViewModel.Queues.Count);
            Assert.AreEqual("Playlists", senders[1].QueuesViewModel.Queues[0].Name);
            Assert.AreEqual("playlists", senders[1].QueuesViewModel.Queues[0].QueueName);
            Assert.AreEqual("octo-laptop", senders[1].QueuesViewModel.Queues[0].Host);
            Assert.AreEqual("user", senders[1].QueuesViewModel.Queues[0].Password);
            Assert.AreEqual("user", senders[1].QueuesViewModel.Queues[0].User);


        }

        [Test]
        public void SetSendersNoSenderSectionLocatorTests()
        {
            ConfigurationToViewModelProvider<SenderConfigurationViewModel> sendersProvider = new ConfigurationToViewModelProvider<SenderConfigurationViewModel>(null);
            var result = sendersProvider.SetViewModelsCollection(null,null);
            Assert.False(result);
        }

        [Test]
        public void SetSendersSenderSectionLocatorNoFilePathTests()
        {
            ConfigurationToViewModelProvider<SenderConfigurationViewModel> sendersProvider = new ConfigurationToViewModelProvider<SenderConfigurationViewModel>(null);
            var result = sendersProvider.SetViewModelsCollection(null, new ConfigurationSectionLocator ());
            Assert.False(result);
        }

        [Test]
        public void SetSendersSenderSectionLocatorWrongFilePathTests()
        {
            ConfigurationToViewModelProvider<SenderConfigurationViewModel> sendersProvider = new ConfigurationToViewModelProvider<SenderConfigurationViewModel>(null);
            Assert.Throws<FileNotFoundException>(
                () => sendersProvider.SetViewModelsCollection(null, new ConfigurationSectionLocator {FilePath = "wrong"}));
        }

        [Test]
        public void SetSendersSenderSectionLocatorNoSectionIdentifierTests()
        {
            ConfigurationToViewModelProvider<SenderConfigurationViewModel> sendersProvider = new ConfigurationToViewModelProvider<SenderConfigurationViewModel>(null);
            Assert.Throws<ConfigurationErrorsException>(
                () =>
                    sendersProvider.SetViewModelsCollection(null,
                        new ConfigurationSectionLocator
                        {
                            FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.config")
                        }));
        }
        [Test]
        public void SetSendersSenderSectionLocatorWrongSectionIdentifierTests()
        {
            ConfigurationToViewModelProvider<SenderConfigurationViewModel> sendersProvider = new ConfigurationToViewModelProvider<SenderConfigurationViewModel>(null);
            Assert.Throws<ConfigurationErrorsException>(
                () =>
                    sendersProvider.SetViewModelsCollection(null,
                        new ConfigurationSectionLocator
                        {
                            FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.config"),
                            SectionIdentifier="wrongSection"
                        }));
        }
        [Test]
        public void SetSendersNoInputTests()
        {
            ConfigurationToViewModelProvider<SenderConfigurationViewModel> sendersProvider = new ConfigurationToViewModelProvider<SenderConfigurationViewModel>(null);
            var result = sendersProvider.SetViewModelsCollection(null,
                new ConfigurationSectionLocator
                {
                    FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test-write.config"),
                    SectionIdentifier = TargetConfigurationDescription.SenderSectionElementName
                });
            Assert.True(result);
            XmlDocument actualDocument = new XmlDocument();
            actualDocument.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"test-write.config"));
            var actualSection = actualDocument.SelectSingleNode($"/{TargetConfigurationDescription.ConfigurationElementName}/{TargetConfigurationDescription.SenderSectionElementName}");
            Assert.IsNotNull(actualSection);
            Assert.False(actualSection.HasChildNodes);
        }

        [Test]
        public void SetSendersNoMapSendersTests()
        {
            ConfigurationToViewModelProvider<SenderConfigurationViewModel> sendersProvider = new ConfigurationToViewModelProvider<SenderConfigurationViewModel>(null);
            var result = sendersProvider.SetViewModelsCollection(new ObservableCollection<SenderConfigurationViewModel>(), new ConfigurationSectionLocator
            {
                FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test-write.config"),
                SectionIdentifier = TargetConfigurationDescription.SenderSectionElementName
            });
            Assert.False(result);
        }

        [Test]
        public void SetSendersMapSendersMapsToNullTests()
        {
            var mockMapSenders =
                MockRepository.Mock<IMap<XmlElement, ObservableCollection<SenderConfigurationViewModel>>>();
            var fakeDoc = new XmlDocument();
            var fakeElement = fakeDoc.CreateElement("fake");
            mockMapSenders.Expect(s => s.UnMap(new ObservableCollection<SenderConfigurationViewModel>(),fakeElement)).IgnoreArguments().Return(null);
            ConfigurationToViewModelProvider<SenderConfigurationViewModel> sendersProvider = new ConfigurationToViewModelProvider<SenderConfigurationViewModel>(mockMapSenders);
            var result = sendersProvider.SetViewModelsCollection(new ObservableCollection<SenderConfigurationViewModel>(), new ConfigurationSectionLocator
            {
                FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test-write.config"),
                SectionIdentifier = TargetConfigurationDescription.SenderSectionElementName
            });
            Assert.False(result);
        }

        [Test]
        public void SetSendersMapSendersMapsToEmptyCollectionTests()
        {
            var mockMapSenders =
                MockRepository.Mock<IMap<XmlElement, ObservableCollection<SenderConfigurationViewModel>>>();
            var fakeDoc = new XmlDocument();
            var fakeElement = fakeDoc.CreateElement(TargetConfigurationDescription.SenderSectionElementName);
            mockMapSenders.Expect(s => s.UnMap(new ObservableCollection<SenderConfigurationViewModel>(),fakeElement)).IgnoreArguments().Return(fakeElement);
            ConfigurationToViewModelProvider<SenderConfigurationViewModel> sendersProvider = new ConfigurationToViewModelProvider<SenderConfigurationViewModel>(mockMapSenders);
            var result = sendersProvider.SetViewModelsCollection(new ObservableCollection<SenderConfigurationViewModel>(), new ConfigurationSectionLocator
            {
                FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test-write.config"),
                SectionIdentifier = TargetConfigurationDescription.SenderSectionElementName
            });
            Assert.False(result);
        }

    }
}
