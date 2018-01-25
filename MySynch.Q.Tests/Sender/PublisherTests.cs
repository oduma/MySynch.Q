using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySynch.Q.Common;
using MySynch.Q.Sender;
using NUnit.Framework;
using RabbitMQ.Client;
using Rhino.Mocks;

namespace MySynch.Q.Tests.Sender
{
    [TestFixture]
    public class PublisherTests
    {
        [Test]
        public void ConstructWithoutSenderQueues()
        {
            try
            {
                var publisher = new Publisher(null,null,null,0);
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual("senderQueues", argumentNullException.ParamName);
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
        public void ConstructWithEmptyListofSenderQueues()
        {
            try
            {
                var publisher = new Publisher(new List<ISenderQueue>().ToArray(), null, null, 0);
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual("senderQueues", argumentNullException.ParamName);
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
        public void ConstructWithoutMessageFeeder()
        {
            try
            {
                var publisher = new Publisher(new List<ISenderQueue>() {new SenderQueue(new QueueElement())}.ToArray(), null,
                    null, 0);
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual("messageFeeder", argumentNullException.ParamName);
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
        public void TryStartAndInitialize()
        {
            var messageFeederMock = MockRepository.GenerateStub<IMessageFeeder>();
            var publisherMock =
                MockRepository.GeneratePartialMock<Publisher>(new ISenderQueue[] {new SenderQueue(new QueueElement())},
                    new List<ConnectionFactory>(), messageFeederMock, 0);
            publisherMock.Stub(m => m.Initialize());
            publisherMock.TryStart();
            
            Assert.True(messageFeederMock.More);
            Assert.IsNotNull(messageFeederMock.PublishMessage);
            Assert.IsNotNull(messageFeederMock.ShouldPublishMessage);
        }
    }
}
