using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FiDa.Models
{
    [Index(nameof(Id), IsUnique = true)]
    public class FileUpload
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

        [Required(ErrorMessage = "Modification Date is required")]
        [DataType(DataType.DateTime, ErrorMessage = "Modification Date must be a valid date and time")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
        public DateTime ModificationDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Creation Date is required")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
        public DateTime CreationDate { get; set; } = DateTime.Now;
    }

    [Index(nameof(Id), nameof(Username), IsUnique = true)]
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; }

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        public virtual List<UserHost> ConfiguredHosts { get; set; } = new List<UserHost>();

        [Required(ErrorMessage = "Modification Date is required")]
        [DataType(DataType.DateTime, ErrorMessage = "Modification Date must be a valid date and time")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime ModificationDate { get; } = new DateTime();
        public DateTime CreatedDate { get; } = new DateTime();
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

        [Required(ErrorMessage = "Modification Date is required")]
        [DataType(DataType.DateTime, ErrorMessage = "Modification Date must be a valid date and time")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime ModificationDate { get; } = new DateTime();
        public DateTime CreatedDate { get; } = new DateTime();
    }

    public enum Hosts
    {
        pCloud,
        Dropbox,
        GoogleDrive,
        OneDrive
    }
}