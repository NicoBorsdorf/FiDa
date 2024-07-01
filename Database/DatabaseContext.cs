using FiDa.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace FiDa.Database
{
    public class FiDaDatabase : DbContext
    {
        public FiDaDatabase(DbContextOptions<FiDaDatabase> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=(LocalDb)\\MSSQLLocalDB;Database=aspnet-53bc9b9d-9d6a-45d4-8429-2a2761773502;Trusted_Connection=true;MultipleActiveResultSets=true");
                //optionsBuilder.UseSqlServer("Data Source=sql_server, 1433;Initial Catalog=FiDaDatabase;User Id=sa;Password=A&VeryComplex123Password;Integrated Security=false;TrustServerCertificate=true;");
            }
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<UploadedFile>(entity =>
        //    {
        //        entity.HasKey(f => f.Id);
        //        entity.HasIndex(f => f.Id).IsUnique();
        //        entity.Property(f => f.Id).ValueGeneratedOnAdd();
        //
        //        entity.Property(f => f.FileName).IsRequired().HasMaxLength(255);
        //
        //        entity.Property(f => f.FileId).IsRequired();
        //
        //        entity.Property(f => f.ParentFolderId).IsRequired();
        //
        //        entity.Property(f => f.Size).HasPrecision(18, 2);
        //
        //        entity.HasOne(f => f.Host).WithMany(h => h.Files).HasForeignKey(f => f.HostId).OnDelete(DeleteBehavior.Restrict);
        //
        //        entity.HasOne(f => f.Account).WithMany(a => a.Files).HasForeignKey(e => e.AccountId).OnDelete(DeleteBehavior.Restrict);
        //
        //        entity.Property(f => f.Modified).ValueGeneratedOnAddOrUpdate().HasDefaultValueSql("GETDATE()");
        //
        //        entity.Property(f => f.Created).ValueGeneratedOnAdd().HasDefaultValueSql("GETDATE()");
        //    });
        //
        //    modelBuilder.Entity<Account>(entity =>
        //    {
        //        entity.HasKey(a => a.Id);
        //        entity.HasIndex(a => new { a.Id, a.Username }).IsUnique();
        //        entity.Property(a => a.Id).ValueGeneratedOnAdd();
        //
        //        entity.Property(a => a.Username).IsRequired();
        //
        //        entity.Property(a => a.Username).IsRequired().HasMaxLength(255);
        //
        //        entity.HasMany(a => a.Files).WithOne(a => a.Account).HasForeignKey(f => f.AccountId).OnDelete(DeleteBehavior.Cascade);
        //
        //        entity.HasMany(a => a.ConfiguredHosts).WithOne(a => a.Account).HasForeignKey(u => u.AccountId).OnDelete(DeleteBehavior.Cascade);
        //
        //        entity.Property(a => a.Modified).ValueGeneratedOnAddOrUpdate().HasDefaultValueSql("GETDATE()");
        //
        //        entity.Property(a => a.Created).ValueGeneratedOnAdd().HasDefaultValueSql("GETDATE()");
        //    });
        //
        //    modelBuilder.Entity<UserHost>(entity =>
        //    {
        //        entity.HasKey(u => u.Id);
        //        entity.HasIndex(u => u.Id).IsUnique();
        //        entity.Property(u => u.Id).ValueGeneratedOnAdd();
        //
        //        entity.Property(u => u.ApiKey).IsRequired();
        //
        //        entity.Property(u => u.Host).IsRequired().HasConversion<int>();
        //
        //        entity.Ignore(u => u.HostAddress);
        //
        //        entity.Property<string>("_uriString").HasColumnName("UriString");
        //
        //        entity.HasOne(a => a.Account).WithMany(a => a.ConfiguredHosts).HasForeignKey(u => u.AccountId).IsRequired().OnDelete(DeleteBehavior.Restrict);
        //
        //        entity.HasMany(u => u.Files).WithOne(f => f.Host).HasForeignKey(f => f.HostId).OnDelete(DeleteBehavior.Cascade);
        //
        //        entity.Property(u => u.Modified).ValueGeneratedOnAddOrUpdate().HasDefaultValueSql("GETDATE()");
        //
        //        entity.Property(u => u.Created).ValueGeneratedOnAdd().HasDefaultValueSql("GETDATE()");
        //        entity.Property(e => e.Str);
        //    });
        //}

        // Db sets
        public DbSet<UploadedFile> UploadedFiles { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<UserHost> UserHost { get; set; }
    }
}