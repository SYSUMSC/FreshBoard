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
        public virtual DbSet<Application> Application { get; set; }
        public virtual DbSet<ApplicationPeriod> ApplicationPeriod { get; set; }
        public virtual DbSet<ApplicationPeriodData> ApplicationPeriodData { get; set; }
        public virtual DbSet<ApplicationPeriodDataType> ApplicationPeriodDataType { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FreshBoardUser>()
                .HasOne(e => e.Application)
                .WithOne(e => e.User);

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
                entity.HasMany(c => c.UserDatas)
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
                    .WithMany(t => t.UserDatas)
                    .HasForeignKey("DataTypeId");

                entity.HasOne(d => d.User)
                    .WithMany(u => u.UserData)
                    .HasForeignKey("UserId");
            });

            modelBuilder.Entity<Application>(entity =>
            {
                entity.HasOne(e => e.Period)
                    .WithMany(e => e.Applications)
                    .HasForeignKey("PeriodId");

                entity.HasOne(e => e.User)
                    .WithOne(e => e.Application)
                    .HasForeignKey<Application>(e => e.UserId);

                entity.HasMany(e => e.Datas)
                    .WithOne(e => e.Application);

                entity.HasKey(e => e.UserId);
            });

            modelBuilder.Entity<ApplicationPeriod>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title).IsRequired().HasDefaultValue(string.Empty);

                entity.Property(e => e.Description).IsRequired().HasDefaultValue(string.Empty);

                entity.Property(e => e.UserApproved).IsRequired().HasDefaultValue(false);

                // entity.Property(e => e.Order).IsRequired().HasDefaultValue(0);

                entity.HasMany(e => e.PeriodDataTypes)
                    .WithOne(e => e.Period);

                entity.HasMany(e => e.Applications)
                    .WithOne(e => e.Period);
            });

            modelBuilder.Entity<ApplicationPeriodData>(entity =>
            {
                entity.Property(e => e.Value).IsRequired().HasDefaultValue(string.Empty);

                entity.HasOne(e => e.Application)
                    .WithMany(e => e.Datas)
                    .HasForeignKey("ApplicationId");

                entity.HasOne(e => e.DataType)
                    .WithMany(e => e.PeriodDatas)
                    .HasForeignKey("DataTypeId");

                entity.HasKey(e => new { e.ApplicationId, e.DataTypeId });
            });

            modelBuilder.Entity<ApplicationPeriodDataType>(entity =>
            {
                entity.Property(e => e.Id).IsRequired();
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title).IsRequired().HasDefaultValue(string.Empty);

                entity.Property(e => e.Description).IsRequired().HasDefaultValue(string.Empty);

                entity.Property(e => e.UserEditable).IsRequired().HasDefaultValue(true);

                entity.Property(e => e.UserVisible).IsRequired().HasDefaultValue(true);

                entity.HasOne(e => e.Period)
                    .WithMany(e => e.PeriodDataTypes)
                    .HasForeignKey("PeriodId");

                entity.HasMany(e => e.PeriodDatas)
                    .WithOne(e => e.DataType);
            });
        }
    }
}
