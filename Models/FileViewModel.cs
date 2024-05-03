using System.ComponentModel.DataAnnotations;
using FiDa.Database;

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
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public double? Size { get; set; }

        [Required(ErrorMessage = "A Parent Folder id is required.")]
        [Range(0, long.MaxValue)]
        public long ParentFolderId { get; set; }

        public bool IsFolder { get; set; } = false;

        [Required(ErrorMessage = "A File Id is required.")]
        [Unique(ErrorMessage = "File Id must be uniqe")]
        public long FileId { get; set; }

        [Required(ErrorMessage = "Modification Date is required")]
        [DataType(DataType.DateTime, ErrorMessage = "Modification Date must be a valid date and time")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
        public DateTime ModificationDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Creation Date is required")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
        public DateTime CreationDate { get; set; } = DateTime.Now;
    }

    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Modification Date is required")]
        [DataType(DataType.DateTime, ErrorMessage = "Modification Date must be a valid date and time")]
        public DateTime ModificationDate { get; set; } = new DateTime();
        public DateTime CreatedDate { get; } = new DateTime();
    }


    // Unique validation
    public class Unique : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            long fileId = long.Parse(value.ToString());
            using (FiDaDatabase db = new())
            {
                return db.UploadedFiles.FirstOrDefault((f) => f.FileId == fileId) == null ? ValidationResult.Success : new ValidationResult(ErrorMessageString);
            }
        }
    }
}