﻿using MySynch.Q.Common.Contracts;
using Sciendo.Common.Logging;
using Sciendo.Common.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MySynch.Q.Common;
using Sciendo.Common.IO;

namespace MySynch.Q.Receiver
{
    public class MessageApplyer
    {
        private readonly string _rootPath;
        private readonly IEnumerable<IMessageTranslator> _messageTranslators;

        public MessageApplyer(string rootPath,IEnumerable<IMessageTranslator> messageTranslators)
        {
            _rootPath = rootPath;
            _messageTranslators = messageTranslators;
        }

        internal void ApplyMessage(byte[] message)
        {
            LoggingManager.Debug("Applying a message...");
            if (message != null && message.Length > 0)
            {
                var transferMessage= Serializer.Deserialize<TransferMessage>(Encoding.UTF8.GetString(message));
                if (transferMessage.Body == null)
                    ApplyDelete(transferMessage.SourceRootPath ,transferMessage.Name);
                else if (transferMessage.BodyType == BodyType.Binary)
                    ApplyBinaryUpSert(transferMessage.SourceRootPath, transferMessage.Name, (byte[]) transferMessage.Body);
                else
                    ApplyTextUpSert(transferMessage.SourceRootPath, transferMessage.Name, (string) transferMessage.Body);
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
