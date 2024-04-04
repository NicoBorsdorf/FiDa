using System.ComponentModel.DataAnnotations;

namespace FiDa.Models
{
    public class FileUpload
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "FileName is required")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "FileName must be between 1 and 255 characters")]
        public required string FileName { get; set; }

        [Required(ErrorMessage = "Host is required")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Host must be between 1 and 255 characters")]
        public required string Host { get; set; }

        [Required(ErrorMessage = "Size is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Size must be a positive number")]
        public double Size { get; set; }

        [Required(ErrorMessage = "ModificationDate is required")]
        [DataType(DataType.DateTime, ErrorMessage = "ModificationDate must be a valid date and time")]
        public DateTime ModificationDate { get; set; }

        [Required(ErrorMessage = "CreatedDate is required")]
        [DataType(DataType.DateTime, ErrorMessage = "CreatedDate must be a valid date and time")]
        public DateTime CreatedDate { get; set; }
    }

    public class User
    {
        [Key]
        public int Id { get; set; }



        [Required(ErrorMessage = "ModificationDate is required")]
        [DataType(DataType.DateTime, ErrorMessage = "ModificationDate must be a valid date and time")]
        public DateTime ModificationDate { get; set; }

        [Required(ErrorMessage = "CreatedDate is required")]
        [DataType(DataType.DateTime, ErrorMessage = "CreatedDate must be a valid date and time")]
        public DateTime CreatedDate { get; set; }
    }
}