using FiDa.DatabaseModels;

namespace FiDa.ViewModels
{
    public class FileViewModel
    {
        public List<UploadedFile> RootFiles { get; set; } = new List<UploadedFile>();
        public List<UploadedFile> Folders { get; set; } = new List<UploadedFile>();
    }

    public class UserViewModel
    {
        public string UserName { get; set; } = null!;
        public string? Email { get; set; }
        public string? ProfilePicture { get; set; }
    }
}