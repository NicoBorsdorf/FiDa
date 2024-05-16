using FiDa.DatabaseModels;

namespace FiDa.ViewModels
{
    public class FileViewModel
    {
        public List<UploadedFile> RootFiles { get; set; }
        public List<UploadedFile> Folders { get; set; }
    }

    public class UserViewModel
    {
        public string UserName { get; set; }
        public string? Email { get; set; }
        public string? ProfilePicture { get; set; }
    }
}