using File_manager.Models;
using System.IO;

namespace File_manager.Interfaces
{
    public interface IStatusEvaluator
    {
        FileStatus CalculateStatus(FileSystemInfo current, AssetMetadata baseline);
    }
}