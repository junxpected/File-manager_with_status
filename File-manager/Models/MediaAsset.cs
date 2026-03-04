using System.IO;
using File_manager.Models;

namespace MediaAsset
{
    public class MediaAsset
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FullPath { get; set; } = string.Empty;
        public string Name => Path.GetFileName(FullPath);
        public FileStatus Status { get; set; } = FileStatus.New;
        public AssetMetadata Baseline { get; set; } = new();
        public string Comment { get; set; } = string.Empty;
    }
}