namespace File_manager.Models
{
    public enum FileStatus
    {
        //Автоматичні стани
        New,
        Modified,
        Locked,
        Missing,

        //Ручні стани
        Approved,
        Rejected,
        Done
    }
}
