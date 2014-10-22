using Sciendo.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySynch.Q.Receiver
{
    public class MessageApplyer
    {
        internal void ApplyMessage(byte[] message)
        {
            if (message != null && message.Length > 0)
            {
                var msg = Encoding.UTF8.GetString(message);
                LoggingManager.Debug(msg + " applied.");
            }
            else
            {
                LoggingManager.Debug("Empty message NOT applied.");
            }

        }
    }
}
