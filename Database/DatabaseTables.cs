using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FiDa.DatabaseModels
{
    [Index(nameof(Id), IsUnique = true)]
    public class UploadedFile
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; }

        [Required(ErrorMessage = "FileName is required")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "FileName must be between 1 and 255 characters")]
        public string FileName { get; set; } = null!;

        [Required(ErrorMessage = "Host is required")]
        public UserHost Host { get; set; } = null!;

        [Range(0, double.MaxValue, ErrorMessage = "Size must be a positive number")]
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public decimal? Size { get; set; }

        [Required(ErrorMessage = "A Parent Folder id is required.")]
        [Range(0, long.MaxValue)]
        public long ParentFolderId { get; set; }

        public bool IsFolder { get; set; } = false;

        [Required(ErrorMessage = "A File Id is required.")]
        public long FileId { get; set; }

        [Required(ErrorMessage = "File can not be created without an set Account")]
        public Account Account { get; set; } = null!;

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
        [DataType(DataType.Date)]
        public DateTime Modified { get; set; } = DateTime.Now;

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
        [DataType(DataType.Date)]
        public DateTime Created { get; set; } = DateTime.Now;
    }

    [Index(nameof(Id), nameof(Username), IsUnique = true)]
    public class Account
    {
        public Account(string username)
        {
            Username = username;
            ConfiguredHosts = new List<UserHost>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; }

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; private set; }

        [ForeignKey("AccountId")]
        public ICollection<UserHost> ConfiguredHosts { get; private set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Modified { get; private set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Created { get; private set; }
    }

    [Index(nameof(Id), IsUnique = true)]
    public class UserHost
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; }

        [Required(ErrorMessage = "A Host Name is required.")]
        public Hosts Host { get; set; }
        public ICollection<UploadedFile> Files { get; private set; } = new List<UploadedFile>();

        [Required(ErrorMessage = "A Api key must be provided.")]
        public string ApiKey { get; set; } = null!;

        [Required(ErrorMessage = "Account is required")]
        public int AccountId { get; set; }
        [Required(ErrorMessage = "Account is required")]
        public Account Account { get; set; } = null!;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Modified { get; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Created { get; }
    }


    public enum Hosts
    {
        PCloud,
        Dropbox,
        GoogleDrive,
        OneDrive
    }
}
