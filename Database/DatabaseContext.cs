using System.Xml;
using FiDa.DatabaseModels;
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
                //optionsBuilder.UseSqlServer("Data Source=(LocalDb)\\MSSQLLocalDB;Database=aspnet-53bc9b9d-9d6a-45d4-8429-2a2761773502;Trusted_Connection=true;MultipleActiveResultSets=true");
                optionsBuilder.UseSqlServer("Data Source=sql_server, 1433;Initial Catalog=FiDaDatabase;User Id=sa;Password=A&VeryComplex123Password;Integrated Security=false;TrustServerCertificate=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UploadedFile>()
                    .Property(s => s.Modified)
                    .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<UploadedFile>()
                   .Property(s => s.Created)
                   .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Account>()
                    .Property(s => s.Modified)
                    .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Account>()
                    .Property(s => s.Created)
                    .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<UserHost>()
                   .Property(s => s.Modified)
                   .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<UserHost>()
                  .Property(s => s.Created)
                  .HasDefaultValueSql("GETDATE()");
        }

        // Db sets
        public DbSet<UploadedFile> UploadedFiles { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<UserHost> UserHost { get; set; }

    }


}