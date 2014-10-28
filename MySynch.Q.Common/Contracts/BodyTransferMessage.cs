using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySynch.Q.Common.Contracts
{
    public class BodyTransferMessage
    {
        public byte[] Body { get; set; }

        public string Name { get; set; }

        public string SourceRootPath { get; set; }
    }
}
