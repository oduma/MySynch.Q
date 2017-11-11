using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySynch.Q.Sender;
using NUnit.Framework;

namespace MySynch.Q.Tests.Sender
{
    [TestFixture]
    public class SenderServiceTests
    {
        [Test]
        public void ConstructWithoutPublisher()
        {
            try
            {
                var senderService  = new SenderService(null);
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual("publisher", argumentNullException.ParamName);
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
