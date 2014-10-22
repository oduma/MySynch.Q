using Sciendo.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MySynch.Q.Sender
{
    public class MessageFeeder
    {
        public MessageFeeder()
        {
            LoggingManager.Debug("Constructing MessageFeeder...");
            LoggingManager.Debug("MessageFeeder Constructed.");

        }

        internal void FeedMessagesTo(Action<byte[]> PublishMessage)
        {
            int i = 0;
            while(More)
            {
                Thread.Sleep(2000);
                PublishMessage(Encoding.UTF8.GetBytes("Message "+ i++ +"."));
            }
            LoggingManager.Debug("Feeder stoped.");
        }

        public bool More { get; set; }
    }
}
