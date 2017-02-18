using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySynch.Q.Common.Contracts;

namespace MySynch.Q.Sender
{
    public interface IIOOperations
    {
        bool FileExists(string path);

        bool IsFileLocked(string path);

        IEnumerable<BodyTransferMessage> GetMessagesFromTheFile(string path, int maxSizePerMessage);
    }
}
