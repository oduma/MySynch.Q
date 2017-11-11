using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySynch.Q.Receiver;
using NUnit.Framework.Internal;
using NUnit.Framework;

namespace MySynch.Q.Tests.Receiver
{
    [TestFixture]
    public class ConsummerTests
    {
        [Test]
        public void ConstructingWithoutReceiverQueue()
        {
            try
            {
                var consummer = new Consummer(null, new MessageApplyer(".."),"abc" );
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual("receiverQueue", argumentNullException.ParamName);
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
        public void ConstructingWithoutMessageApplyer()
        {
            try
            {
                var consummer = new Consummer(new ReceiverQueue(), null, "abc");
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual("messageApplyer", argumentNullException.ParamName);
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
        public void ConstructingWithoutrootPath ()
        {
            try
            {
                var consummer = new Consummer(new ReceiverQueue(), new MessageApplyer(".."), null);
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
                var consummer = new Consummer(new ReceiverQueue(), new MessageApplyer(".."), "..\abc");
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

    }

}
