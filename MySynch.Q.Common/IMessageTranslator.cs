using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySynch.Q.Common
{
    public interface IMessageTranslator
    {
        byte[] Translate(byte[] inBytes, Dictionary<string, string> findReplacePairs);
    }
}
