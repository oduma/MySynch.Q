using System.Configuration;

namespace MySynch.Q.Receiver.Configuration
{
    
    public class ReceiversSection:ConfigurationSection
    {
        [ConfigurationProperty("receivers")]
        public ReceiverElementCollection Receivers
        {
            get { return (ReceiverElementCollection)this["receivers"]; }
        }
    }
 }
