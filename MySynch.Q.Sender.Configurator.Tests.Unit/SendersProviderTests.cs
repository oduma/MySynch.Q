using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MySynch.Q.Common.Contracts;
using MySynch.Q.Sender.Configurator.Configuration;
using MySynch.Q.Sender.Configurator.Mappers;
using MySynch.Q.Sender.Configurator.Models;
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
            SendersProvider sendersProvider = new SendersProvider(null);
            var sendersCollection = sendersProvider.GetSenders(null);
            Assert.IsNotNull(sendersCollection);
            Assert.IsEmpty(sendersCollection);
        }

        [Test]
        public void GetSendersNoSenderLocatorFilePathTests()
        {
            SendersProvider sendersProvider = new SendersProvider(null);
            var sendersCollection = sendersProvider.GetSenders(new SenderSectionLocator());
            Assert.IsNotNull(sendersCollection);
            Assert.IsEmpty(sendersCollection);
        }

        [Test]
        public void GetSendersSenderLocatorFilePathNotValidTests()
        {
            SendersProvider sendersProvider = new SendersProvider(null);
            Assert.Throws<FileNotFoundException>(() => sendersProvider.GetSenders(new SenderSectionLocator {FilePath = "aaa"}));
        }

        [Test]
        public void GetSendersNoMapSendersTests()
        {
            SendersProvider sendersProvider = new SendersProvider(null);
            var sendersCollection = sendersProvider.GetSenders(new SenderSectionLocator
            {
                FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.config")
            });
            Assert.IsNotNull(sendersCollection);
            Assert.IsEmpty(sendersCollection);
        }

        [Test]
        public void GetSendersSenderLocatorEmptySectionIdentifierTests()
        {

            SendersProvider sendersProvider =
                new SendersProvider(
                    new MapSenders(new MapSender(new MapFilters(new MapFilter()), new MapQueues(new MapQueue()))));
            Assert.Throws<ConfigurationErrorsException>(
                () =>
                    sendersProvider.GetSenders(new SenderSectionLocator
                    {
                        FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.config")
                    }));
        }
        [Test]
        public void GetSendersSenderLocatorNotFoundSectionIdentifierTests()
        {

            SendersProvider sendersProvider =
                new SendersProvider(
                    new MapSenders(new MapSender(new MapFilters(new MapFilter()), new MapQueues(new MapQueue()))));
            Assert.Throws<ConfigurationErrorsException>(
                () =>
                    sendersProvider.GetSenders(new SenderSectionLocator
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
            SendersProvider sendersProvider = new SendersProvider(mockMapSenders);
            var sendersResult = sendersProvider.GetSenders(new SenderSectionLocator
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
            SendersProvider sendersProvider =
                new SendersProvider(
                    new MapSenders(new MapSender(new MapFilters(new MapFilter()), new MapQueues(new MapQueue()))));
            var senders = sendersProvider.GetSenders(new SenderSectionLocator
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
            SendersProvider sendersProvider = new SendersProvider(null);
            var result = sendersProvider.SetSenders(null,null);
            Assert.False(result);
        }

        [Test]
        public void SetSendersSenderSectionLocatorNoFilePathTests()
        {
            SendersProvider sendersProvider = new SendersProvider(null);
            var result = sendersProvider.SetSenders(null, new SenderSectionLocator ());
            Assert.False(result);
        }

        [Test]
        public void SetSendersSenderSectionLocatorWrongFilePathTests()
        {
            SendersProvider sendersProvider = new SendersProvider(null);
            Assert.Throws<FileNotFoundException>(
                () => sendersProvider.SetSenders(null, new SenderSectionLocator {FilePath = "wrong"}));
        }

        [Test]
        public void SetSendersSenderSectionLocatorNoSectionIdentifierTests()
        {
            SendersProvider sendersProvider = new SendersProvider(null);
            Assert.Throws<ConfigurationErrorsException>(
                () =>
                    sendersProvider.SetSenders(null,
                        new SenderSectionLocator
                        {
                            FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.config")
                        }));
        }
        [Test]
        public void SetSendersSenderSectionLocatorWrongSectionIdentifierTests()
        {
            SendersProvider sendersProvider = new SendersProvider(null);
            Assert.Throws<ConfigurationErrorsException>(
                () =>
                    sendersProvider.SetSenders(null,
                        new SenderSectionLocator
                        {
                            FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.config"),
                            SectionIdentifier="wrongSection"
                        }));
        }
        [Test]
        public void SetSendersNoInputTests()
        {
            SendersProvider sendersProvider = new SendersProvider(null);
            var result = sendersProvider.SetSenders(null,
                new SenderSectionLocator
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
            SendersProvider sendersProvider = new SendersProvider(null);
            var result = sendersProvider.SetSenders(new ObservableCollection<SenderConfigurationViewModel>(), new SenderSectionLocator
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
            SendersProvider sendersProvider = new SendersProvider(mockMapSenders);
            var result = sendersProvider.SetSenders(new ObservableCollection<SenderConfigurationViewModel>(), new SenderSectionLocator
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
            SendersProvider sendersProvider = new SendersProvider(mockMapSenders);
            var result = sendersProvider.SetSenders(new ObservableCollection<SenderConfigurationViewModel>(), new SenderSectionLocator
            {
                FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test-write.config"),
                SectionIdentifier = TargetConfigurationDescription.SenderSectionElementName
            });
            Assert.False(result);
        }

    }
}
