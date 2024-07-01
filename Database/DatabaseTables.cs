using FiDa.Database;
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
        public string FileName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Host is required")]
        public int HostId { get; set; }
        [ForeignKey(nameof(HostId))]
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public UserHost Host { get; set; } = null!;

        [Range(0, double.MaxValue, ErrorMessage = "Size must be a positive number")]
        [Precision(18, 2)]
        public decimal? Size { get; set; }

        [Required(ErrorMessage = "A Parent Folder id is required.")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "A Parent Folder id is required.")]
        public string ParentFolderId { get; set; } = string.Empty;

        public bool IsFolder { get; set; } = false;

        [Required(ErrorMessage = "A File Id is required.")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "A File Id is required.")]
        public string FileId { get; set; } = string.Empty;

        [Required(ErrorMessage = "File can not be created without an set Account")]
        public int AccountId { get; set; } // Foreign key property
        [ForeignKey(nameof(AccountId))]
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Account Account { get; set; } = null!;

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
        [DataType(DataType.Date)]
        public DateTime Modified { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
        [DataType(DataType.Date)]
        public DateTime Created { get; set; }
    }

    [Index(nameof(Id), nameof(Username), IsUnique = true)]
    public class Account
    {
        public Account(string username)
        {
            Username = username;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; }

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        public ICollection<UserHost> ConfiguredHosts { get; } = new List<UserHost>();

        public ICollection<UploadedFile> Files { get; } = new List<UploadedFile>();

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DataType(DataType.Date)]
        public DateTime Modified { get; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DataType(DataType.Date)]
        public DateTime Created { get; }
    }

    [Index(nameof(Id), IsUnique = true)]
    public class UserHost
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; }

        [Required(ErrorMessage = "A Host Name is required.")]
        public Hosts Host { get; set; }
        public ICollection<UploadedFile> Files { get; } = new List<UploadedFile>();

        [Required(ErrorMessage = "A Api key must be provided.")]
        public string ApiKey { get; set; } = string.Empty;

        [NotMapped]
        public Uri? HostAddress
        {
            get => _uriString != null ? new(_uriString) : null;
            set => _uriString = value?.ToString();
        }

        [Column("UriString")]
        private string? _uriString;

        [Required(ErrorMessage = "Account is required")]
        public int AccountId { get; set; } // Foreign key property
        [ForeignKey(nameof(AccountId))]
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Account Account { get; set; } = null!;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DataType(DataType.Date)]
        public DateTime Modified { get; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DataType(DataType.Date)]
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
