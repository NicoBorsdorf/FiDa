using FiDa.Models;
using Microsoft.EntityFrameworkCore;

namespace FiDa.Database
{


    public class FiDaDatabase : DbContext
    {
        public FiDaDatabase(DbContextOptions<FiDaDatabase> options) : base(options)
        {
        }

        public FiDaDatabase()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=sql_server,1433;Initial Catalog=FiDaDatabase;User Id=sa;Password=A&VeryComplex123Password;Integrated Security=false;TrustServerCertificate=true;");
                //optionsBuilder.UseSqlite(@"Data Source =.\\Data\\SQlLiteDatabase.db");
            }
        }

        // Db sets
        public DbSet<FileUpload> UploadedFiles { get; set; }
        public DbSet<User> User { get; set; }
    }


    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new FiDaDatabase(
                serviceProvider.GetRequiredService<
                    DbContextOptions<FiDaDatabase>>());

            //check if db has been created
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Look for any FileUploads.
            if (context.UploadedFiles.Any())
            {
                return;   // DB has been seeded
            }
            context.UploadedFiles.AddRange(
                new FileUpload
                {
                    FileName = "My first file",
                    Host = "pCloud",
                    Size = 13.23,
                    ModificationDate = DateTime.Now,
                    CreatedDate = DateTime.Now,
                },
                new FileUpload
                {
                    FileName = "My second file",
                    Host = "pCloud",
                    Size = 85.66,
                    ModificationDate = DateTime.Now,
                    CreatedDate = DateTime.Now,
                },
                new FileUpload
                {
                    FileName = "My third file",
                    Host = "pCloud",
                    Size = 23.40,
                    ModificationDate = DateTime.Now,
                    CreatedDate = DateTime.Now,
                },
                new FileUpload
                {
                    FileName = "My fourth file",
                    Host = "pCloud",
                    Size = 1.56,
                    ModificationDate = DateTime.Now,
                    CreatedDate = DateTime.Now,
                }
            );
            context.SaveChanges();
        }
    }
}