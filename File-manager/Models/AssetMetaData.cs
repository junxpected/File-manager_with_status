namespace File_manager.Models
{
    public class AssetMetadata
    {
        public long RegisteredSize { get; set; }
        public DateTime RegisteredTime { get; set; }
        public List<string> Tags { get; set; } = new();
    }
}