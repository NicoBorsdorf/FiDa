using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FiDa.DatabaseModels
{
    [Index(nameof(Id), IsUnique = true)]
    public class UploadedFile
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; }

        [Required(ErrorMessage = "FileName is required")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "FileName must be between 1 and 255 characters")]
        public string FileName { get; set; }

        [Required(ErrorMessage = "Host is required")]
        public Hosts Hostname { get; set; }

        [Required(ErrorMessage = "Size is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Size must be a positive number")]
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public double? Size { get; set; }

        [Required(ErrorMessage = "A Parent Folder id is required.")]
        [Range(0, long.MaxValue)]
        public long ParentFolderId { get; set; }

        public bool IsFolder { get; set; } = false;

        [Required(ErrorMessage = "A File Id is required.")]
        public long FileId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
        public DateTime ModificationDate { get; set; } = DateTime.Now;

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
        public DateTime CreationDate { get; set; } = DateTime.Now;
    }

    [Index(nameof(Id), nameof(Username), IsUnique = true)]
    public class User
    {
        [Required(ErrorMessage = "User Id is required")]
        public string Id { get; private set; }

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        public virtual List<UserHost> ConfiguredHosts { get; set; } = new List<UserHost>();

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime ModificationDate { get; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedDate { get; }
    }

    [Index(nameof(Id), IsUnique = true)]
    public class UserHost
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; }

        [Required(ErrorMessage = "A Host Name is required.")]
        public Hosts Hostname { get; set; }

        [Required(ErrorMessage = "A Api key must be provided.")]
        public string ApiKey { get; set; }

        public virtual User User { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime ModificationDate { get; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedDate { get; }
    }

    public enum Hosts
    {
        pCloud,
        Dropbox,
        GoogleDrive,
        OneDrive
    }
}
