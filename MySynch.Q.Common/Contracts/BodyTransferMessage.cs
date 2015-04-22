using System;

namespace MySynch.Q.Common.Contracts
{
    public class BodyTransferMessage
    {
        public byte[] Body { get; set; }

        public string Name { get; set; }

        public string SourceRootPath { get; set; }

        public Guid MessageId { get; set; }
    }
}
