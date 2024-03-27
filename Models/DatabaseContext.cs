using FiDa.Models;
using Microsoft.EntityFrameworkCore;

namespace FiDa.Database
{


    public class FiDaDatabase : DbContext
    {
        private readonly IConfiguration _configuration;

        public FiDaDatabase(DbContextOptions<FiDaDatabase> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("FiDaDatabase"));
            }
        }

        // Db sets
        public DbSet<FileUpload> UploadedFiles { get; set; }
    }


    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new FiDaDatabase(
                serviceProvider.GetRequiredService<
                    DbContextOptions<FiDaDatabase>>());
            // Look for any FileUploads.
            if (context.UploadedFiles.Any())
            {
                return;   // DB has been seeded
            }
            context.UploadedFiles.AddRange(
                new FileUpload
                {
                    ID = 1,
                    FileName = "My first file",
                    Host = "pCloud",
                    Size = 13.23,
                    ModificationDate = new DateTime(),
                    CreatedDate = new DateTime(),
                },
                new FileUpload
                {
                    ID = 2,
                    FileName = "My second file",
                    Host = "pCloud",
                    Size = 85.66,
                    ModificationDate = new DateTime(),
                    CreatedDate = new DateTime(),
                },
                new FileUpload
                {
                    ID = 3,
                    FileName = "My third file",
                    Host = "pCloud",
                    Size = 23.40,
                    ModificationDate = new DateTime(),
                    CreatedDate = new DateTime(),
                },
                new FileUpload
                {
                    ID = 4,
                    FileName = "My fourth file",
                    Host = "pCloud",
                    Size = 1.56,
                    ModificationDate = new DateTime(),
                    CreatedDate = new DateTime(),
                }
            );
            context.SaveChanges();
        }
    }
}