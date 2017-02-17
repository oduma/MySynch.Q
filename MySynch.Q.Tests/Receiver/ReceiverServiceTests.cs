 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using MySynch.Q.Receiver;
 using NUnit.Framework;

namespace MySynch.Q.Tests.Receiver
{
    [TestFixture ]
    public class ReceiverServiceTests
    {
        [Test]
        public void ConstructWithoutConsummer()
        {
            try
            {
                var receiverService= new ReceiverService(null);
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual("consummer", argumentNullException.ParamName);
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
