using MySynch.Q.Common;
using MySynch.Q.Common.Contracts;
using Sciendo.Common.Logging;
using System;
using System.IO;
using System.Threading;
using System.Web;
using Sciendo.Common.IO;

namespace MySynch.Q.Sender
{
    public class MessageFeeder
    {
        private readonly BodyType _messageBodyType;

        private void fsWatcher_Renamed(string oldPath, string newPath)
        {
            if (!More)
            {
                StopFeeder();
                return;
            }
            LoggingManager.Debug("A File renamed from " + oldPath + " to " + newPath);
            //if it is a directory ignore it
            if (!File.Exists(newPath))
                return;
            // Wait if file is still open
            FileInfo fileInfo = new FileInfo(newPath);
            while (IsFileLocked(fileInfo))
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
            PublishMessage(new TransferMessage { BodyType = _messageBodyType, Name = oldPath, Body = null,SourceRootPath=RootPath });
            PublishMessage(new TransferMessage { BodyType = _messageBodyType, Name = newPath, Body = GetFileContent(newPath),SourceRootPath=RootPath });

        }

        private object GetFileContent(string filePath)
        {
            if (_messageBodyType == BodyType.Binary)
            {
                return GetBinaryFileContent(filePath);
            }
            if (_messageBodyType == BodyType.Text)
                return  new TextFileReader().Read(filePath);
            return null;
        }

        private byte[] GetBinaryFileContent(string filePath )
        {

            FileInfo fInfo = new FileInfo(filePath);

            byte[] buffer = new byte[fInfo.Length];
            using (var fs = File.OpenRead(filePath))
            {
                fs.Read(buffer, 0, (int)fInfo.Length);
                return buffer;
            }

        }

        private void fsWatcher_Deleted(string path)
        {
            if (!More)
            {
                StopFeeder();
                return;
            }
            LoggingManager.Debug("A file deleted: " + path);
            if (Directory.Exists(path))
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
            PublishMessage(new TransferMessage { BodyType = _messageBodyType, Name = path, Body = null, SourceRootPath = RootPath });
        }

        private void StopFeeder()
        {
            if(_fsWatcher!=null)
            {
                _fsWatcher.Change -= fsWatcher_Changed;
                _fsWatcher.Delete -= fsWatcher_Deleted;
                _fsWatcher.Rename -= fsWatcher_Renamed;
                _fsWatcher.Stop();

            }
        }

        private void fsWatcher_Changed(string path)
        {
            if (!More)
            {
                StopFeeder();
                return;
            }
            LoggingManager.Debug("A file changed: " + path);
            //if it is a directory ignore it
            if (!File.Exists(path))
                return;
            //queue an insert;
            // Wait if file is still open
            FileInfo fileInfo = new FileInfo(path);
            while (IsFileLocked(fileInfo))
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
            PublishMessage(new TransferMessage { BodyType = _messageBodyType, Name = path, Body = GetFileContent(path), SourceRootPath = RootPath });
        }

        static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                if((file.Attributes & FileAttributes.ReadOnly)==FileAttributes.ReadOnly)
                    stream = file.Open(FileMode.Open,
                         FileAccess.Read, FileShare.None);
                else
                    stream = file.Open(FileMode.Open,
                         FileAccess.ReadWrite, FileShare.None);

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
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        private readonly DirectoryMonitor _fsWatcher;
        public string RootPath { get; private set; }


        public MessageFeeder(string localRootFolder, BodyType messageBodyType, params string[] filterExtensions)
        {
            _messageBodyType = messageBodyType;
            LoggingManager.Debug("Constructing _messageFeeder...");
            if (string.IsNullOrEmpty(localRootFolder))
                throw new ArgumentNullException(nameof(localRootFolder));
            if (!Directory.Exists(localRootFolder))
                throw new ArgumentException("localRootFolder does not exist");

            _fsWatcher = new DirectoryMonitor(localRootFolder);
            RootPath = localRootFolder;

            StopFeeder();
            _fsWatcher.Change += fsWatcher_Changed;
            _fsWatcher.Delete += fsWatcher_Deleted;
            _fsWatcher.Rename += fsWatcher_Renamed;
            _fsWatcher.Start();
            LoggingManager.Debug("_messageFeeder Constructed.");

        }

        public Action<TransferMessage> PublishMessage { get; set; }

        public Func<bool> ShouldPublishMessage { get; set; } 

        public bool More { get; set; }

        public void Initialize(bool acceptMessages, Action<TransferMessage> publishMessage, Func<bool> shouldPublishMessage)
        {
            More = acceptMessages;
            PublishMessage = publishMessage;
            ShouldPublishMessage = shouldPublishMessage;
        }
    }
}
