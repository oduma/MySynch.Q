using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySynch.Q.Common.Contracts.Management
{
    public class VersionMessage:DetailMessageBase
    {
        public string version { get; set; }
    }
}
