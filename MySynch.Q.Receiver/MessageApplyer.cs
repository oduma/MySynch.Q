using MySynch.Q.Common.Contracts;
using Sciendo.Common.Logging;
using Sciendo.Common.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MySynch.Q.Receiver
{
    public class MessageApplyer
    {
        private readonly string _rootPath;
        public MessageApplyer(string rootPath)
        {
            _rootPath = rootPath;
        }

        internal void ApplyMessage(byte[] message)
        {
            LoggingManager.Debug("Applying a message...");
            if (message != null && message.Length > 0)
            {
                var msgWithBody= Serializer.Deserialize<BodyTransferMessage>(Encoding.UTF8.GetString(message));
                if (msgWithBody.Body == null)
                    ApplyDelete(msgWithBody.SourceRootPath ,msgWithBody.Name);
                else if (msgWithBody.Part.FromParts == 1)
                {
                    ApplyUpSert(msgWithBody.SourceRootPath, msgWithBody.Name, msgWithBody.Body);
                }
                else if(msgWithBody.Part.FromParts>msgWithBody.Part.PartId)
                {
                    ApplyUpSert(msgWithBody.SourceRootPath,
                        string.Format("{0}.part{1}", msgWithBody.Name, msgWithBody.Part.PartId), msgWithBody.Body);
                }
                else if (msgWithBody.Part.PartId > 1 && msgWithBody.Part.PartId == msgWithBody.Part.FromParts)
                {
                    ApplyUpSert(msgWithBody.SourceRootPath,
                        string.Format("{0}.part{1}", msgWithBody.Name, msgWithBody.Part.PartId), msgWithBody.Body);
                    GatherParts(msgWithBody.Name, msgWithBody.SourceRootPath);
                }
                LoggingManager.Debug("Message applied.");
            }
            else
            {
                LoggingManager.Debug("Empty message NOT applied.");
            }
        }

        private void GatherParts(string name,string sourceRootPath)
        {
            LoggingManager.Debug("Gathering all the info in the file " + name);
            var localFileName = _rootPath + name.Replace(sourceRootPath, "");
            if(File.Exists(localFileName))
                File.Delete(localFileName);
            var localFolder = Path.GetDirectoryName(localFileName);
            using (var fs = File.Create(localFileName))
            {
                var orderedParts = GetPartsInOrder(localFolder, localFileName);
                foreach (var key in orderedParts.Keys)
                {

                    var fLen = new FileInfo(orderedParts[key]).Length;
                    byte[] buffer = new byte[fLen];
                    using (var fr = File.OpenRead(orderedParts[key]))
                    {
                        fr.Read(buffer, 0, (int) fLen);
                    }
                    File.Delete(orderedParts[key]);
                    fs.Write(buffer, 0, (int) fLen);
                }
            }
        }

        private SortedList<int,string> GetPartsInOrder(string localFolder, string localFileName)
        {
            SortedList<int, string> result = new SortedList<int, string>();
            foreach (var file in Directory.GetFiles(localFolder, "*.part*").Where(f => f.Contains(localFileName)))
            {
                result.Add(Convert.ToInt32(file.Replace(string.Format("{0}.part",localFileName),"")),file);
            }
            return result;
        }

        private void ApplyUpSert(string sourceRootPath, string name, byte[] body)
        {
            LoggingManager.Debug("Applying upsert from " + sourceRootPath + " to " + _rootPath + " of " + name);

            try
            {
                var localFileName = _rootPath+name.Replace(sourceRootPath, "");
                var localFolder = Path.GetDirectoryName(localFileName);
                if(!Directory.Exists(localFolder))
                    Directory.CreateDirectory(localFolder);
                using (var fs = File.Create(localFileName, body.Length))
                {
                    fs.Write(body, 0, body.Length);
                }
                LoggingManager.Debug("Upsert applied.");
            }
            catch(Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
            }

        }

        private void ApplyDelete(string sourceRootPath, string name)
        {
            LoggingManager.Debug("Applying delete to " + _rootPath + " of " + name);
            try
            {
                var localDeleteFileName = name.Replace(
                    sourceRootPath, _rootPath);
                LoggingManager.Debug("Transformed name: " + localDeleteFileName);
                if (File.Exists(localDeleteFileName))
                    File.Delete(localDeleteFileName);
                if(Directory.Exists(localDeleteFileName))
                    Directory.Delete(localDeleteFileName,true);
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
            }
        }
    }
}
