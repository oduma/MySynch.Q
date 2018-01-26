using System.Configuration;

namespace MySynch.Q.Sender.Configuration
{
    
    public class SendersSection:ConfigurationSection
    {
        [ConfigurationProperty("senders")]
        public SenderElementCollection Senders
        {
            get { return (SenderElementCollection) this["senders"]; }
        }
    }
}
