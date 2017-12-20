using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySynch.Q.Sender.Configurator.Configuration
{
    public static class TargetSenderConfigurationDescription
    {
        public static string LocalRootFolder => "localRootFolder";
        public static string MessageBodyType => "messageBodyType";
        public static string MinMemory => "minMem";
        public static string SenderElementName => "add";
        public static string SendersCollectionElementName => "senders";
        public static string SectionElementName => "sendersSection";
    }
}
