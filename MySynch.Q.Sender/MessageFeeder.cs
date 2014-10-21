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
            while(More)
            {
                Thread.Sleep(2000);
                PublishMessage(new byte[0]);
            }
            LoggingManager.Debug("Feeder stoped.");
        }

        public bool More { get; set; }
    }
}
