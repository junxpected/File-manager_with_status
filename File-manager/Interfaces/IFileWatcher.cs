using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace File_manager.Interfaces
{
    public interface IFileWatcher
    {
        event Action <FileSystemEventArgs> OnFileSystemChanged;
        void Start(string path);
        void Stop();
    }
}
