using MySynch.Q.Common;
using MySynch.Q.Common.Contracts;
using Sciendo.Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace MySynch.Q.Sender
{
    public class MessageFeeder
    {
        private readonly int _maxFileSize;

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
            PublishMessage(new BodyTransferMessage { Name = oldPath, Body = null,SourceRootPath=_rootPath });
            foreach (var message in BuildMessage(newPath))
            {
                PublishMessage(message);
            }
        }

        private IEnumerable<BodyTransferMessage> BuildMessage(string filePath)
        {

            FileInfo fInfo = new FileInfo(filePath);

            var totalParts= (_maxFileSize == 0 || _maxFileSize >= fInfo.Length) ? 1 : fInfo.Length/_maxFileSize +1;
            var oneReadLength = (totalParts==1) ? fInfo.Length : _maxFileSize;
            using (var fs = File.OpenRead(filePath))
            {
                int i = 1;
                while (fs.Position < fInfo.Length)
                {
                    var remainingBytes = fInfo.Length - fs.Position;
                    if (oneReadLength > remainingBytes)
                        oneReadLength = remainingBytes;
                    byte[] buffer = new byte[oneReadLength];
                    fs.Read(buffer, 0, (int) oneReadLength);
                    yield return
                        new BodyTransferMessage
                        {
                            Name = filePath,
                            Body = buffer,
                            Part = new PartInfo {PartId = i++, FromParts = (int) totalParts},
                            SourceRootPath = _rootPath
                        };
                }
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
            PublishMessage(new BodyTransferMessage { Name = path, Body = null, SourceRootPath = _rootPath });
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
            foreach (var message in BuildMessage(path))
            {
                PublishMessage(message);
            }

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

        private DirectoryMonitor _fsWatcher;
        private string _rootPath;

        public void Initialize(string localRootFolder)
        {
            LoggingManager.Debug(localRootFolder + " Initializing _messageFeeder...");

            if (string.IsNullOrEmpty(localRootFolder))
                throw new ArgumentNullException("localRootFolder");
            if (!Directory.Exists(localRootFolder))
                throw new ArgumentException("localRootFolder does not exist");

            _fsWatcher = new DirectoryMonitor(localRootFolder);
            _rootPath = localRootFolder;

            StopFeeder();
            _fsWatcher.Change += fsWatcher_Changed;
            _fsWatcher.Delete += fsWatcher_Deleted;
            _fsWatcher.Rename += fsWatcher_Renamed;
            _fsWatcher.Start();
            LoggingManager.Debug(localRootFolder + " Initialized _messageFeeder...");
        }

        public MessageFeeder(int maxFileSize)
        {
            LoggingManager.Debug(string.Format("Constructing _messageFeeder {0} {1} ...",
                (maxFileSize == 0) ? "without" : "with", (maxFileSize == 0) ? "any limit" : maxFileSize + " limit"));
            _maxFileSize = maxFileSize;
            LoggingManager.Debug("_messageFeeder Constructed.");

        }

        public Action<BodyTransferMessage> PublishMessage { get; set; }

        public Func<bool> ShouldPublishMessage { get; set; } 

        public bool More { get; set; }
    }
}
