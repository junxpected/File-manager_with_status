using System.IO;
using File_manager.Models;
using File_manager.Interfaces;

namespace File_manager.Models
{
    public class MediaAsset : IAsset
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FullPath { get; set; } = string.Empty;
        public string Name => Path.GetFileName(FullPath);
        public FileStatus Status { get; set; } = FileStatus.New;
        public AssetMetadata Baseline { get; set; } = new();
        public string Comment { get; set; } = string.Empty;
    }
}