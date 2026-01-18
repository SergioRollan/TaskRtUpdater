using Microsoft.EntityFrameworkCore;
using TaskRtUpdater.src.Domain;

namespace TaskRtUpdater.src.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<TaskEntity> Tasks => Set<TaskEntity>();
        public DbSet<TaskDependency> Dependencies => Set<TaskDependency>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TaskEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.Status).HasConversion<string>();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.ModifiedAt).IsRequired();
                entity.Property(e => e.Priority).IsRequired();
                entity.Property(e => e.Duration).IsRequired();

                entity.HasMany(e => e.Dependencies)
                    .WithOne(d => d.Task)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TaskDependency>(entity =>
            {
                entity.HasKey(td => new { td.TaskId, td.DependencyId });
                
                entity.HasOne(td => td.Task)
                    .WithMany(t => t.Dependencies)
                    .HasForeignKey(td => td.TaskId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(td => td.Dependency)
                    .WithMany()
                    .HasForeignKey(td => td.DependencyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(td => new { td.TaskId, td.DependencyId }).IsUnique();
            });
        }
    }
}
