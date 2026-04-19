using System;
using System.IO;
using File_manager.Interfaces;

namespace File_manager.Services
{
    public class FileSystemMonitor : IFileWatcher, IDisposable
    {
        private FileSystemWatcher? _watcher;

        public event Action<FileSystemEventArgs>? OnFileSystemChanged;

        public void Start(string path)
        {
            Stop();

            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
                throw new DirectoryNotFoundException($"Folder not found: {path}");

            _watcher = new FileSystemWatcher(path)
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.Size
            };

            _watcher.Created += OnCreated;
            _watcher.Changed += OnChanged;
            _watcher.Deleted += OnDeleted;
            _watcher.Renamed += OnRenamed;
            _watcher.Error += OnError;
            _watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            if (_watcher == null) return;

            _watcher.EnableRaisingEvents = false;
            _watcher.Created -= OnCreated;
            _watcher.Changed -= OnChanged;
            _watcher.Deleted -= OnDeleted;
            _watcher.Renamed -= OnRenamed;
            _watcher.Error -= OnError;
            _watcher.Dispose();
            _watcher = null;
        }

        private void OnCreated(object sender, FileSystemEventArgs e) => OnFileSystemChanged?.Invoke(e);
        private void OnChanged(object sender, FileSystemEventArgs e) => OnFileSystemChanged?.Invoke(e);
        private void OnDeleted(object sender, FileSystemEventArgs e) => OnFileSystemChanged?.Invoke(e);
        private void OnRenamed(object sender, RenamedEventArgs e) => OnFileSystemChanged?.Invoke(e);

        private void OnError(object sender, ErrorEventArgs e)
        {
            // Найчастіше пов'язано з лімітом спостереження. Перезапустимо, коли зміни з'являться.
            try
            {
                if (_watcher != null && !string.IsNullOrWhiteSpace(_watcher.Path))
                {
                    var path = _watcher.Path;
                    Stop();
                    Start(path);
                }
            }
            catch { }
        }

        public void Dispose() => Stop();
    }
}
