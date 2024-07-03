using FiDa.DatabaseModels;

namespace FiDa.ViewModels
{
    public class BaseViewModel
    {
        public Account Account { get; set; } = null!;
        public FileViewModel? FileViewModel { get; set; }
    }

    public class FileViewModel
    {
        public List<UploadedFile> RootFiles { get; set; } = new List<UploadedFile>();
        public List<UploadedFile> Folders { get; set; } = new List<UploadedFile>();
    }

    public class ErrorViewModel
    {
        public string RequestId { get; set; }
    }

    public class StorageUsageModel
    {
        public UserHost Host { get; set; }
        public double Free { get; set; }
        public double Used { get; set; }
    }
}