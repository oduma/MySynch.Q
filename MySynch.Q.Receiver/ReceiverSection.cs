using System.Configuration;

namespace MySynch.Q.Receiver
{
    
    public class ReceiverSection:ConfigurationSection
    {
        [ConfigurationProperty("receivers")]
        public ReceiverElementCollection Receivers
        {
            get { return (ReceiverElementCollection)this["receivers"]; }
        }
    }
 }
