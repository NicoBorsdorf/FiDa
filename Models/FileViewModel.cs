using System.ComponentModel.DataAnnotations;

namespace FiDa.Models
{
    public class FileUpload
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "FileName is required")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "FileName must be between 1 and 255 characters")]
        public string FileName { get; set; }

        [Required(ErrorMessage = "Host is required")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Host must be between 1 and 255 characters")]
        public string Host { get; set; }

        [Required(ErrorMessage = "Size is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Size must be a positive number")]
        public double Size { get; set; }
        [Required(ErrorMessage = "A Parent Folder id is required.")]
        [Range(0, long.MaxValue)]
        public long ParentFolderId { get; set; }

        public DateTime ModificationDate { get; set; } = new DateTime();

        [Required(ErrorMessage = "CreatedDate is required")]
        [DataType(DataType.DateTime, ErrorMessage = "CreatedDate must be a valid date and time")]
        public DateTime CreatedDate { get; set; }
    }

    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        public DateTime ModificationDate { get; set; } = new DateTime();

        [Required(ErrorMessage = "CreatedDate is required")]
        [DataType(DataType.DateTime, ErrorMessage = "CreatedDate must be a valid date and time")]
        public DateTime CreatedDate { get; set; }
    }
}