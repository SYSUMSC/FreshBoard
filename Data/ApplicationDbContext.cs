using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using mscfreshman.Data.Identity;

namespace mscfreshman.Data
{
    public class ApplicationDbContext : IdentityDbContext<FreshBoardUser>
    {
        private readonly string _connectionString;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public ApplicationDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public virtual DbSet<Notification> Notification { get; set; }
        public virtual DbSet<ReadStatus> ReadStatus { get; set; }
        public virtual DbSet<Problem> Problem { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(_connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Content).IsRequired();

                entity.Property(e => e.Mode).HasDefaultValueSql("1");

                entity.Property(e => e.Time).IsRequired();

                entity.Property(e => e.Title).IsRequired();

                entity.Property(e => e.HasPushed).IsRequired().HasDefaultValueSql("0");
            });

            modelBuilder.Entity<ReadStatus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.UserId).IsRequired();

                entity.Property(e => e.NotificationId).IsRequired();
            });

            modelBuilder.Entity<Problem>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Title).IsRequired();

                entity.Property(e => e.Content).IsRequired();

                entity.Property(e => e.Level).IsRequired();
            });
        }
    }
}
