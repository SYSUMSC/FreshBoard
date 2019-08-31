using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using mscfreshman.Data.Identity;

namespace mscfreshman.Data
{
    public class DbContext : IdentityDbContext<FreshBoardUser>
    {
        private readonly string _connectionString;
        public DbContext(DbContextOptions<DbContext> options)
            : base(options)
        {
        }

        public DbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public virtual DbSet<Notification> Notification { get; set; }
        public virtual DbSet<ReadStatus> ReadStatus { get; set; }
        public virtual DbSet<Problem> Problem { get; set; }
        public virtual DbSet<CrackRecord> CrackRecord { get; set; }
        public virtual DbSet<UserDataType> UserDataType { get; set; }
        public virtual DbSet<UserData> UserData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(_connectionString);
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

                entity.Property(e => e.HasPushed).IsRequired().HasDefaultValue(false);
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

            modelBuilder.Entity<CrackRecord>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Time).IsRequired();

                entity.Property(e => e.UserId).IsRequired();

                entity.Property(e => e.ProblemId).IsRequired();

                entity.Property(e => e.Result).IsRequired();
            });

            modelBuilder.Entity<UserDataType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Title).IsRequired();
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasDefaultValue(string.Empty);
                entity.HasMany(c => c.UserData)
                    .WithOne(u => u.DataType);

                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<UserData>(entity =>
            {
                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasDefaultValue(string.Empty);

                entity.HasKey(e => new { e.UserId, e.DataTypeId });

                entity.HasOne(d => d.DataType)
                    .WithMany(t => t.UserData)
                    .HasForeignKey("DataTypeId");

                entity.HasOne(d => d.User)
                    .WithMany(u => u.UserData)
                    .HasForeignKey("UserId");
            });
        }
    }
}
