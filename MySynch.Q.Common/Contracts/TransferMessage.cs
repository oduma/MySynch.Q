using System;

namespace MySynch.Q.Common.Contracts
{
    public class TransferMessage
    {
        public BodyType BodyType { get; set; }
        public object Body { get; set; }

        public string Name { get; set; }

        public string SourceRootPath { get; set; }

        public Guid MessageId { get; set; }
    }
}
