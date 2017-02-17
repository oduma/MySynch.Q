using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySynch.Q.Common.Contracts;
using MySynch.Q.Receiver;
using NUnit.Framework;
using Rhino.Mocks;

namespace MySynch.Q.Tests.Receiver
{
    [TestFixture]
    public class MessageApplyerTests
    {
        [Test]
        public void ConstructingWithoutrootPath()
        {
            try
            {
                var messageApplyer = new MessageApplyer(null);
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual("rootPath", argumentNullException.ParamName);
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
        public void ConstructingWithInexistentRootPath()
        {
            try
            {
                var messageApplyer = new MessageApplyer("..\abc");
            }
            catch (ArgumentException argumentException)
            {
                Assert.AreEqual("rootPath", argumentException.ParamName);
                Assert.AreEqual("Root Path not found.\r\nParameter name: rootPath", argumentException.Message);
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
        public void ApplyMessageWithoutAName()
        {
            try
            {
                var messageApplyer = new MessageApplyer("..");
                messageApplyer.ApplyMessage(new BodyTransferMessage ());
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual("Name", argumentNullException.ParamName);
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
        public void ApplyMessageWithoutSourceRootPath()
        {
            try
            {
                var messageApplyer = new MessageApplyer("..");
                messageApplyer.ApplyMessage(new BodyTransferMessage {Name="abc"});
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual("SourceRootPath", argumentNullException.ParamName);
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
        public void ApplyMessageForDelete()
        {
            var messageApplyerMocked = MockRepository.GenerateStrictMock<MessageApplyer>("..");
            messageApplyerMocked.Expect(m => m.ApplyDelete("abc", "abc"));
            messageApplyerMocked.ApplyMessage(new BodyTransferMessage { Name = "abc", SourceRootPath = "abc"});
            messageApplyerMocked.VerifyAllExpectations();
        }

        [Test]
        public void ApplyMessageForUpsertWithoutPartsDefintion()
        {
            var messageApplyerMocked = MockRepository.GenerateStrictMock<MessageApplyer>("..");
            messageApplyerMocked.ApplyMessage(new BodyTransferMessage { Name = "abc", SourceRootPath = "abc", Part=null, Body= new byte[2] {0,0} });
            
            messageApplyerMocked.VerifyAllExpectations();
        }

        [Test]
        public void Apply1of1PartsMessageForUpsert()
        {
            var message = new BodyTransferMessage
            {
                Name = "abc",
                SourceRootPath = "abc",
                Body = new byte[2] {0, 0},
                Part = new PartInfo {PartId = 1, FromParts = 1}
            };
            var messageApplyerMocked = MockRepository.GenerateStrictMock<MessageApplyer>("..");
            messageApplyerMocked.Expect(m => m.ApplyUpSert("abc", "abc",message.Body));
            messageApplyerMocked.ApplyMessage(message);
            messageApplyerMocked.VerifyAllExpectations();
        }
        [Test]
        public void ApplyNofMMessageForUpsert()
        {
            var message = new BodyTransferMessage
            {
                Name = "abc",
                SourceRootPath = "abc",
                Body = new byte[2] { 0, 0 },
                Part = new PartInfo { PartId = 2, FromParts = 3 }
            };
            var messageApplyerMocked = MockRepository.GenerateStrictMock<MessageApplyer>("..");
            messageApplyerMocked.Expect(m => m.ApplyUpSert("abc", "abc.part2", message.Body));
            messageApplyerMocked.ApplyMessage(message);
            messageApplyerMocked.VerifyAllExpectations();
        }
        [Test]
        public void ApplyMofMMessageForUpsert()
        {
            var message = new BodyTransferMessage
            {
                Name = "abc",
                SourceRootPath = "abc",
                Body = new byte[2] { 0, 0 },
                Part = new PartInfo { PartId = 3, FromParts = 3 }
            };
            var messageApplyerMocked = MockRepository.GenerateStrictMock<MessageApplyer>("..");
            messageApplyerMocked.Expect(m => m.ApplyUpSert("abc", "abc.part3", message.Body));
            messageApplyerMocked.Expect(m => m.GatherParts("abc", "abc"));
            messageApplyerMocked.ApplyMessage(message);
            messageApplyerMocked.VerifyAllExpectations();
        }

    }
}
