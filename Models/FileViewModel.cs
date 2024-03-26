using System;
using System.Data.Entity;

namespace FiDa.Models
{
    public class FileUpload
    {

        public required int ID { get; set; }
        public required string FileName { get; set; }
        public required string Host { get; set; }
        public required double Size { get; set; }
        public required DateTime ModificationDate { get; set; }
        public required DateTime CreatedDate { get; set; }
    }

    public class UploadedFileDBContext : DbContext
    {
        public DbSet<FileUpload> UploadedFiles { get; set; }
    }
}