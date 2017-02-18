using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySynch.Q.Common;
using MySynch.Q.Common.Contracts;
using MySynch.Q.Sender;
using NUnit.Framework.Internal;
using NUnit.Framework;
using Rhino.Mocks;

namespace MySynch.Q.Tests.Sender
{
    [TestFixture]
    public class MessageFeederTests
    {
        [Test]
        public void ConstructWithoutDirectoryMonitor()
        {
            try
            {
                var messageFeeder = new MessageFeeder(0,null,null, null);
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual("directoryMonitor", argumentNullException.ParamName);
                return;
            }
            catch (Exception exception)
            {
                Assert.Fail("Wrong exception type.");
                return;
            }
            Assert.Fail("No exception thrown.");
        }

        [Test]
        public void ConstructWithoutLocalRootFolder()
        {
            try
            {
                var messageFeeder = new MessageFeeder(0, new DirectoryMonitor(".."), null,null);
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual("localRootFolder", argumentNullException.ParamName);
                return;
            }
            catch (Exception exception)
            {
                Assert.Fail("Wrong exception type.");
                return;
            }
            Assert.Fail("No exception thrown.");
        }

        [Test]
        public void FileRenamedStoppedOk()
        {
            var directoryMonitorMock = MockRepository.GenerateMock<IDirectoryMonitor>();
            directoryMonitorMock.Expect(m => m.Stop());
            
            var messageFeeder = new MessageFeeder(0, directoryMonitorMock, "abc", new IOOperations());
            messageFeeder.More = false;
            messageFeeder.FileRenamed("..","..");
            directoryMonitorMock.VerifyAllExpectations();
        }
        [Test]
        public void FileRenamedNoNewFile()
        {
            var ioOperationsMock = MockRepository.GenerateStrictMock<IIOOperations>();
            ioOperationsMock.Expect(m => m.FileExists("def")).Return(false);

            var messageFeeder = new MessageFeeder(0, new DirectoryMonitor(".."), "abc", ioOperationsMock);
            messageFeeder.More = true;
            messageFeeder.FileRenamed("abc", "def");
            ioOperationsMock.VerifyAllExpectations();
        }
        [Test]
        public void FileRenamedNoShouldPublish()
        {
            var ioOperationsMock = MockRepository.GenerateStrictMock<IIOOperations>();
            ioOperationsMock.Expect(m => m.FileExists("def")).Return(true);
            ioOperationsMock.Expect(m => m.IsFileLocked("def")).Return(false);

            var messageFeeder = new MessageFeeder(0, new DirectoryMonitor(".."), "abc", ioOperationsMock);
            messageFeeder.More = true;
            messageFeeder.FileRenamed("abc", "def");
            ioOperationsMock.VerifyAllExpectations();
        }

        public bool WillPublishMessage()
        {
            return true;
        }

        public void PublishOnePartMessage(BodyTransferMessage message)
        {
            if (message.Body == null)
            {
                Assert.AreEqual(message.Name, "abc");
                Assert.AreEqual(message.SourceRootPath, "abc");
                Assert.IsNull(message.Part);
            }
            else
            {
                Assert.AreEqual(message.Part.PartId,1);
                Assert.AreEqual(message.Part.FromParts,1);
                Assert.AreEqual(message.Name,"def");
                Assert.AreEqual(message.SourceRootPath,"abc");
            }            
        }

        public void PublishThreePartMessage(BodyTransferMessage message)
        {
            if (message.Body == null)
            {
                Assert.AreEqual(message.Name, "abc");
                Assert.AreEqual(message.SourceRootPath, "abc");
                Assert.IsNull(message.Part);
            }
            else
            {
                Assert.GreaterOrEqual(message.Part.PartId, 1);
                Assert.LessOrEqual(message.Part.PartId, 3);
                Assert.AreEqual(message.Part.FromParts, 3);
                Assert.AreEqual(message.Name, "def");
                Assert.AreEqual(message.SourceRootPath, "abc");
            }
        }
        [Test]
        public void FileRenamedPublishOneMessage()
        {
            var ioOperationsMock = MockRepository.GenerateStrictMock<IIOOperations>();
            ioOperationsMock.Expect(m => m.FileExists("def")).Return(true);
            ioOperationsMock.Expect(m => m.IsFileLocked("def")).Return(false);
            ioOperationsMock.Expect(m => m.GetMessagesFromTheFile("def", 0))
                .Return(new[]
                {
                    new BodyTransferMessage
                    {
                        Body = new byte[] {0, 0},
                        Name = "def",
                        Part = new PartInfo {FromParts = 1, PartId = 1},
                        SourceRootPath = "abc"
                    }
                });

            var messageFeeder = new MessageFeeder(0, new DirectoryMonitor(".."), "abc", ioOperationsMock);
            messageFeeder.More = true;
            messageFeeder.ShouldPublishMessage = WillPublishMessage;
            messageFeeder.PublishMessage = PublishOnePartMessage;
            messageFeeder.FileRenamed("abc", "def");
            ioOperationsMock.VerifyAllExpectations();
        }

        [Test]
        public void FileRenamedPublishMoreMessage()
        {
            var ioOperationsMock = MockRepository.GenerateStrictMock<IIOOperations>();
            ioOperationsMock.Expect(m => m.FileExists("def")).Return(true);
            ioOperationsMock.Expect(m => m.IsFileLocked("def")).Return(false);
            ioOperationsMock.Expect(m => m.GetMessagesFromTheFile("def", 0))
                .Return(new[]
                {
                    new BodyTransferMessage
                    {
                        Body = new byte[] {0, 0},
                        Name = "def",
                        Part = new PartInfo {FromParts = 3, PartId = 1},
                        SourceRootPath = "abc"
                    },
                                        new BodyTransferMessage
                    {
                        Body = new byte[] {0, 0},
                        Name = "def",
                        Part = new PartInfo {FromParts = 3, PartId = 2},
                        SourceRootPath = "abc"
                    },
                    new BodyTransferMessage
                    {
                        Body = new byte[] {0, 0},
                        Name = "def",
                        Part = new PartInfo {FromParts = 3, PartId = 3},
                        SourceRootPath = "abc"
                    }

                });

            var messageFeeder = new MessageFeeder(0, new DirectoryMonitor(".."), "abc", ioOperationsMock);
            messageFeeder.More = true;
            messageFeeder.ShouldPublishMessage = WillPublishMessage;
            messageFeeder.PublishMessage = PublishThreePartMessage;
            messageFeeder.FileRenamed("abc", "def");
            ioOperationsMock.VerifyAllExpectations();
        }

        [Test]
        public void FileDeletedStoppedOk()
        {
            var directoryMonitorMock = MockRepository.GenerateMock<IDirectoryMonitor>();
            directoryMonitorMock.Expect(m => m.Stop());

            var messageFeeder = new MessageFeeder(0, directoryMonitorMock, "abc", new IOOperations());
            messageFeeder.More = false;
            messageFeeder.FileDeleted("..");
            directoryMonitorMock.VerifyAllExpectations();
        }
        [Test]
        public void FileDeletedNoFile()
        {
            var ioOperationsMock = MockRepository.GenerateStrictMock<IIOOperations>();
            ioOperationsMock.Expect(m => m.FileExists("def")).Return(false);

            var messageFeeder = new MessageFeeder(0, new DirectoryMonitor(".."), "abc", ioOperationsMock);
            messageFeeder.More = true;
            messageFeeder.FileDeleted("def");
            ioOperationsMock.VerifyAllExpectations();
        }

        public void PublishDeleteMessage(BodyTransferMessage message)
        {
            Assert.IsNull(message.Body);
            Assert.AreEqual(message.Name, "def");
            Assert.AreEqual(message.SourceRootPath, "abc");
            Assert.IsNull(message.Part);
        }

        [Test]
        public void FileDeletedPublishOneMessage()
        {
            var ioOperationsMock = MockRepository.GenerateStrictMock<IIOOperations>();
            ioOperationsMock.Expect(m => m.FileExists("def")).Return(true);

            var messageFeeder = new MessageFeeder(0, new DirectoryMonitor(".."), "abc", ioOperationsMock);
            messageFeeder.More = true;
            messageFeeder.ShouldPublishMessage = WillPublishMessage;
            messageFeeder.PublishMessage = PublishDeleteMessage;
            messageFeeder.FileDeleted("def");
            ioOperationsMock.VerifyAllExpectations();
        }

        [Test]
        public void FileChangedStoppedOk()
        {
            var directoryMonitorMock = MockRepository.GenerateMock<IDirectoryMonitor>();
            directoryMonitorMock.Expect(m => m.Stop());

            var messageFeeder = new MessageFeeder(0, directoryMonitorMock, "abc", new IOOperations());
            messageFeeder.More = false;
            messageFeeder.FileChanged("..");
            directoryMonitorMock.VerifyAllExpectations();
        }
        [Test]
        public void FileChangedNoNewFile()
        {
            var ioOperationsMock = MockRepository.GenerateStrictMock<IIOOperations>();
            ioOperationsMock.Expect(m => m.FileExists("def")).Return(false);

            var messageFeeder = new MessageFeeder(0, new DirectoryMonitor(".."), "abc", ioOperationsMock);
            messageFeeder.More = true;
            messageFeeder.FileChanged("def");
            ioOperationsMock.VerifyAllExpectations();
        }
        [Test]
        public void FileChangedNoShouldPublish()
        {
            var ioOperationsMock = MockRepository.GenerateStrictMock<IIOOperations>();
            ioOperationsMock.Expect(m => m.FileExists("def")).Return(true);
            ioOperationsMock.Expect(m => m.IsFileLocked("def")).Return(false);

            var messageFeeder = new MessageFeeder(0, new DirectoryMonitor(".."), "abc", ioOperationsMock);
            messageFeeder.More = true;
            messageFeeder.FileChanged("def");
            ioOperationsMock.VerifyAllExpectations();
        }


        [Test]
        public void FileChangedPublishOneMessage()
        {
            var ioOperationsMock = MockRepository.GenerateStrictMock<IIOOperations>();
            ioOperationsMock.Expect(m => m.FileExists("def")).Return(true);
            ioOperationsMock.Expect(m => m.IsFileLocked("def")).Return(false);
            ioOperationsMock.Expect(m => m.GetMessagesFromTheFile("def", 0))
                .Return(new[]
                {
                    new BodyTransferMessage
                    {
                        Body = new byte[] {0, 0},
                        Name = "def",
                        Part = new PartInfo {FromParts = 1, PartId = 1},
                        SourceRootPath = "abc"
                    }
                });

            var messageFeeder = new MessageFeeder(0, new DirectoryMonitor(".."), "abc", ioOperationsMock);
            messageFeeder.More = true;
            messageFeeder.ShouldPublishMessage = WillPublishMessage;
            messageFeeder.PublishMessage = PublishOnePartMessage;
            messageFeeder.FileChanged("def");
            ioOperationsMock.VerifyAllExpectations();
        }

        [Test]
        public void FileChangedPublishMoreMessage()
        {
            var ioOperationsMock = MockRepository.GenerateStrictMock<IIOOperations>();
            ioOperationsMock.Expect(m => m.FileExists("def")).Return(true);
            ioOperationsMock.Expect(m => m.IsFileLocked("def")).Return(false);
            ioOperationsMock.Expect(m => m.GetMessagesFromTheFile("def", 0))
                .Return(new[]
                {
                    new BodyTransferMessage
                    {
                        Body = new byte[] {0, 0},
                        Name = "def",
                        Part = new PartInfo {FromParts = 3, PartId = 1},
                        SourceRootPath = "abc"
                    },
                                        new BodyTransferMessage
                    {
                        Body = new byte[] {0, 0},
                        Name = "def",
                        Part = new PartInfo {FromParts = 3, PartId = 2},
                        SourceRootPath = "abc"
                    },
                    new BodyTransferMessage
                    {
                        Body = new byte[] {0, 0},
                        Name = "def",
                        Part = new PartInfo {FromParts = 3, PartId = 3},
                        SourceRootPath = "abc"
                    }

                });

            var messageFeeder = new MessageFeeder(0, new DirectoryMonitor(".."), "abc", ioOperationsMock);
            messageFeeder.More = true;
            messageFeeder.ShouldPublishMessage = WillPublishMessage;
            messageFeeder.PublishMessage = PublishThreePartMessage;
            messageFeeder.FileChanged("def");
            ioOperationsMock.VerifyAllExpectations();
        }

    }
}
