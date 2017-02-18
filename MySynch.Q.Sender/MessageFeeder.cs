using MySynch.Q.Common;
using MySynch.Q.Common.Contracts;
using Sciendo.Common.Logging;
using System;
using System.Threading;

namespace MySynch.Q.Sender
{
    public class MessageFeeder : IMessageFeeder
    {
        private readonly int _maxFileSize;

        internal virtual void FileRenamed(string oldPath, string newPath)
        {
            if (!More)
            {
                StopFeeder();
                return;
            }
            LoggingManager.Debug("A File renamed from " + oldPath + " to " + newPath);
            //if it is a directory ignore it
            if (!_ioOperations.FileExists(newPath))
                return;
            // Wait if file is still open
            while (_ioOperations.IsFileLocked(newPath))
            {
                Thread.Sleep(500);
            }
            if (ShouldPublishMessage == null)
            {
                LoggingManager.Debug("Queue checker inactive. Will not publish anything.");
                return;
            }
            while (!ShouldPublishMessage())
            {
                LoggingManager.Debug("Waiting 5 seconds queue is busy...");
                Thread.Sleep(5000);
            }
            PublishMessage(new BodyTransferMessage { Name = oldPath, Body = null,SourceRootPath=_rootPath });
            foreach (var message in _ioOperations.GetMessagesFromTheFile(newPath,_maxFileSize))
            {
                message.SourceRootPath = _rootPath;
                PublishMessage(message);
            }
        }

        internal virtual void FileDeleted(string path)
        {
            if (!More)
            {
                StopFeeder();
                return;
            }
            LoggingManager.Debug("A file deleted: " + path);
            if (!_ioOperations.FileExists(path))
                return;
            if (ShouldPublishMessage == null)
            {
                LoggingManager.Debug("Queue checker inactive. Will not publish anything.");
                return;
            }
            while (!ShouldPublishMessage())
            {
                LoggingManager.Debug("Waiting 5 seconds queue is busy...");
                Thread.Sleep(5000);
            }
            PublishMessage(new BodyTransferMessage { Name = path, Body = null, SourceRootPath = _rootPath });
        }

        private void StopFeeder()
        {
            if(_directoryMonitor!=null)
            {
                _directoryMonitor.Change -= FileChanged;
                _directoryMonitor.Delete -= FileDeleted;
                _directoryMonitor.Rename -= FileRenamed;
                _directoryMonitor.Stop();

            }
        }

        internal virtual void FileChanged(string path)
        {
            if (!More)
            {
                StopFeeder();
                return;
            }
            LoggingManager.Debug("A file changed: " + path);
            //if it is a directory ignore it
            if (!_ioOperations.FileExists(path))
                return;
            //queue an insert;
            // Wait if file is still open
            while (_ioOperations.IsFileLocked(path))
            {
                Thread.Sleep(500);
            }
            if (ShouldPublishMessage == null)
            {
                LoggingManager.Debug("Queue checker inactive. Will not publish anything.");
                return;
            }
            //queue an update;
            while (!ShouldPublishMessage())
            {
                LoggingManager.Debug("Waiting 5 seconds queue is busy...");
                Thread.Sleep(5000);
            }
            foreach (var message in _ioOperations.GetMessagesFromTheFile(path,_maxFileSize))
            {
                message.SourceRootPath = _rootPath;
                PublishMessage(message);
            }

        }


        private IDirectoryMonitor _directoryMonitor;
        private string _rootPath;
        private readonly IIOOperations _ioOperations;

        public void Initialize()
        {
            LoggingManager.Debug(_rootPath + " Initializing _messageFeeder...");

            StopFeeder();
            _directoryMonitor.Change += FileChanged;
            _directoryMonitor.Delete += FileDeleted;
            _directoryMonitor.Rename += FileRenamed;
            _directoryMonitor.Start();
            LoggingManager.Debug(_rootPath + " Initialized _messageFeeder...");
        }

        public MessageFeeder(int maxFileSize, IDirectoryMonitor directoryMonitor, string localRootFolder,IIOOperations ioOperations)
        {
            if(directoryMonitor==null)
                throw new ArgumentNullException(nameof(directoryMonitor));
            if(string.IsNullOrEmpty(localRootFolder))
                throw new ArgumentNullException(nameof(localRootFolder));
            LoggingManager.Debug(string.Format("Constructing _messageFeeder {0} {1} ...",
                (maxFileSize == 0) ? "without" : "with", (maxFileSize == 0) ? "any limit" : maxFileSize + " limit"));
            _directoryMonitor = directoryMonitor;
            _rootPath = localRootFolder;
            _ioOperations = (ioOperations)??new IOOperations();
            _maxFileSize = maxFileSize;
            LoggingManager.Debug("_messageFeeder Constructed.");

        }

        public virtual Action<BodyTransferMessage> PublishMessage { get; set; }

        public virtual Func<bool> ShouldPublishMessage { get; set; } 

        public virtual bool More { get; set; }
    }
}
