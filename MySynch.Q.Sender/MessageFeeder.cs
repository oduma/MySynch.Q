﻿using MySynch.Q.Common;
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
            //queue an insert;
            // Wait if file is still open
            FileInfo fileInfo = new FileInfo(newPath);
            while (IsFileLocked(fileInfo))
            {
                Thread.Sleep(500);
            }
            PublishMessage(new BodyTransferMessage { Name = oldPath, Body = null,SourceRootPath=_rootPath });
            PublishMessage(new BodyTransferMessage { Name = newPath, Body = GetFileContent(newPath),SourceRootPath=_rootPath });

        }

        private byte[] GetFileContent(string filePath)
        {
            FileInfo fInfo = new FileInfo(filePath);

            byte[] buffer = new byte[fInfo.Length];
            using(var fs = File.OpenRead(filePath))
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
            //queue an update;
            PublishMessage(new BodyTransferMessage { Name = path, Body = GetFileContent(path), SourceRootPath = _rootPath });
        }

        static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
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

        public MessageFeeder()
        {
            LoggingManager.Debug("Constructing _messageFeeder...");
            LoggingManager.Debug("_messageFeeder Constructed.");

        }

        public Action<BodyTransferMessage> PublishMessage { get; set; }

        public bool More { get; set; }
    }
}
