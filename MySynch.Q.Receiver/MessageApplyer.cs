using MySynch.Q.Common.Contracts;
using Sciendo.Common.Logging;
using Sciendo.Common.Serialization;
using System;
using System.IO;
using System.Text;
using Sciendo.Common.IO;

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
                var msgWithBody= Serializer.Deserialize<TransferMessage>(Encoding.UTF8.GetString(message));
                if (msgWithBody.Body == null)
                    ApplyDelete(msgWithBody.SourceRootPath ,msgWithBody.Name);
                else if (msgWithBody.BodyType == BodyType.Binary)
                    ApplyBinaryUpSert(msgWithBody.SourceRootPath, msgWithBody.Name, (byte[]) msgWithBody.Body);
                else
                    ApplyTextUpSert(msgWithBody.SourceRootPath, msgWithBody.Name, (string) msgWithBody.Body);
                LoggingManager.Debug("Message applied.");
            }
            else
            {
                LoggingManager.Debug("Empty message NOT applied.");
            }
        }

        private void ApplyTextUpSert(string sourceRootPath, string name, string body)
        {
            LoggingManager.Debug("Applying upsert from " + sourceRootPath + " to " + _rootPath + " of " + name);

            //apply all transformations on the messagebody

            try
            {
                var localFileName = GetLocalFileName(sourceRootPath, name);
                if (body.Length == 0)
                    File.WriteAllText(localFileName, string.Empty);
                else
                    new TextFileWriter().Write(body,localFileName);

                LoggingManager.Debug("Upsert applied.");
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
            }
        }

        private void ApplyBinaryUpSert(string sourceRootPath, string name, byte[] body)
        {
            LoggingManager.Debug("Applying upsert from " + sourceRootPath + " to " + _rootPath + " of " + name);

            //apply all transformations on the messagebody

            try
            {
                var localFileName = GetLocalFileName(sourceRootPath, name);
                if(body.Length==0)
                    File.WriteAllText(localFileName,string.Empty);
                else
                {
                    using (var fs = File.Create(localFileName, body.Length))
                    {
                        fs.Write(body, 0, body.Length);
                    }
                }
                LoggingManager.Debug("Upsert applied.");
            }
            catch(Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
            }

        }

        private string GetLocalFileName(string sourceRootPath, string name)
        {
            var localFileName = _rootPath + name.Replace(sourceRootPath, "");
            var localFolder = Path.GetDirectoryName(localFileName);
            if (!Directory.Exists(localFolder))
                Directory.CreateDirectory(localFolder);
            return localFileName;
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
