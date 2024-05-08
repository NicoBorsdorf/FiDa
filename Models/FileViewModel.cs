using FiDa.DatabaseModels;

namespace FiDa.ViewModels
{
    public class FileViewModel
    {
        public string Id { get; set; }

        public string FileName { get; set; }

        public Hosts Hostname { get; set; }

        public double? Size { get; set; }

        public long ParentFolderId { get; set; }

        public bool IsFolder { get; set; } = false;

        public long FileId { get; set; }

        public DateTime ModificationDate { get; set; } = DateTime.Now;

        public DateTime CreationDate { get; set; } = DateTime.Now;
    }

    public class UserViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string ProfilePicture { get; set; }
    }
}