using MySynch.Q.Common.Contracts;
using Sciendo.Common.Logging;
using Sciendo.Common.Serialization;
using System;
using System.IO;
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
                else
                    ApplyUpSert(msgWithBody.SourceRootPath, msgWithBody.Name, msgWithBody.Body);
                LoggingManager.Debug("Message applied.");
            }
            else
            {
                LoggingManager.Debug("Empty message NOT applied.");
            }
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
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
            }
        }
    }
}
