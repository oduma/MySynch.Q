using MySynch.Q.Common.Contracts;
using Sciendo.Common.Logging;
using Sciendo.Common.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sciendo.Common.IO;
using Sciendo.Common.Music.Contracts;
using Sciendo.Playlist.Translator;

namespace MySynch.Q.Receiver
{
    public class MessageApplyer
    {
        private readonly string _rootPath;
        private readonly IEnumerable<ITranslator> _translators;
        private readonly IEnumerable<IPostProcessor> _postProcessors;
        private readonly IStorage _storage;

        public MessageApplyer(string rootPath,
            IEnumerable<ITranslator> translators,
            IEnumerable<IPostProcessor> postProcessors,
            IStorage storage)

        {
            _rootPath = rootPath;
            _translators = translators;
            _postProcessors = postProcessors;
            _storage = storage;
        }

        internal void ApplyMessage(byte[] message)
        {
            LoggingManager.Debug("Applying a message...");
            if (message != null && message.Length > 0)
            {
                LoggingManager.Debug("Message not null trying to deserialize it...");
                try
                {
                    var transferMessage = Serializer.Deserialize<TransferMessage>(Encoding.UTF8.GetString(message));
                    LoggingManager.Debug($"Message deserialized:{transferMessage.Name}");
                    if (transferMessage.Body == null)
                        ApplyDelete(transferMessage.SourceRootPath, transferMessage.Name);
                    else if (transferMessage.BodyType == BodyType.Binary)
                        ApplyBinaryUpSert(transferMessage.SourceRootPath, transferMessage.Name, (byte[])transferMessage.Body);
                    else
                        ApplyTextUpSert(transferMessage.SourceRootPath, transferMessage.Name, TranslateMessageBody((string)transferMessage.Body));
                    LoggingManager.Debug("Message applied.");
                    PostProcessMessage(transferMessage);
                    LoggingManager.Debug("Finished with message.");
                }
                catch (Exception e)
                {
                    LoggingManager.Debug("Exception occured during applying the message. NOt Applied. See error logs.");
                    LoggingManager.LogSciendoSystemError(e);
                }
            }
            else
            {
                LoggingManager.Debug("Empty message NOT applied.");
            }
        }

        private void PostProcessMessage(TransferMessage transferMessage)
        {
            LoggingManager.Debug("Starting Postprocessing of message...");
            foreach (var postProcessor in _postProcessors)
            {
                if(postProcessor!=null && transferMessage!=null && transferMessage.Body!=null)
                    postProcessor.Process(transferMessage.Body,transferMessage.Name);
            }
            LoggingManager.Debug("PostProcessing of message finished.");
        }

        private string TranslateMessageBody(string transferMessage)
        {
            foreach (var messageTranslator in _translators)
            {
                transferMessage = messageTranslator.Translate(transferMessage);
            }
            return transferMessage;
        }

        private void ApplyTextUpSert(string sourceRootPath, string name, string body)
        {
            LoggingManager.Debug("Applying upsert from " + sourceRootPath + " to " + _rootPath + " of " + name);

            try
            {
                var localFileName = GetLocalFileName(sourceRootPath, name);
                _storage.File.WriteAllText(localFileName,(body.Length==0)?string.Empty:body);

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

            try
            {
                var localFileName = GetLocalFileName(sourceRootPath, name);
                if(body.Length==0)
                    _storage.File.WriteAllText(localFileName,string.Empty);
                else
                {
                    _storage.File.Create(localFileName, body);
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
            if (!_storage.Directory.Exists(localFolder))
                _storage.Directory.CreateDirectory(localFolder);
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
                if (_storage.File.Exists(localDeleteFileName))
                {
                    _storage.File.Delete(localDeleteFileName);
                    return;
                }
                if(_storage.Directory.Exists(localDeleteFileName))
                    _storage.Directory.Delete(localDeleteFileName,true);
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
            }
        }
    }
}
