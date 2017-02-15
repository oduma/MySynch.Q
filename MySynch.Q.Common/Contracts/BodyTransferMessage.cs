using System;

namespace MySynch.Q.Common.Contracts
{
    public class BodyTransferMessage
    {
        public byte[] Body { get; set; }

        public string Name { get; set; }

        public string SourceRootPath { get; set; }

        public PartInfo Part { get; set; }
    }

    public class PartInfo
    {
        public int PartId { get; set; }
        public int FromParts { get; set; }
    }
}
