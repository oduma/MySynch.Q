using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySynch.Q.Common.Contracts;

namespace MySynch.Q.Sender
{
    public class IOOperations:IIOOperations
    {
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public bool IsFileLocked(string path)
        {
            FileInfo file = new FileInfo(path);

            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open,
                    (file.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly
                        ? FileAccess.Read
                        : FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                stream?.Close();
            }

            //file is not locked
            return false;
        }

        public IEnumerable<BodyTransferMessage> GetMessagesFromTheFile(string path, int maxSizePerMessage)
        {
            FileInfo fInfo = new FileInfo(path);

            var totalParts = (maxSizePerMessage == 0 || maxSizePerMessage >= fInfo.Length) ? 1 : fInfo.Length / maxSizePerMessage + 1;
            var oneReadLength = (totalParts == 1) ? fInfo.Length : maxSizePerMessage;
            using (var fs = File.OpenRead(path))
            {
                int i = 1;
                while (fs.Position < fInfo.Length)
                {
                    var remainingBytes = fInfo.Length - fs.Position;
                    if (oneReadLength > remainingBytes)
                        oneReadLength = remainingBytes;
                    byte[] buffer = new byte[oneReadLength];
                    fs.Read(buffer, 0, (int)oneReadLength);
                    yield return
                        new BodyTransferMessage
                        {
                            Name = path,
                            Body = buffer,
                            Part = new PartInfo { PartId = i++, FromParts = (int)totalParts },
                        };
                }
            }

        }
    }
}
