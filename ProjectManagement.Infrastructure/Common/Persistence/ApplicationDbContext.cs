using Microsoft.EntityFrameworkCore;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Common.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Project> Projects => Set<Project>();
        public DbSet<TaskItem> TaskItems => Set<TaskItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // RN-04: Filtro Global para Soft Delete de TaskItems
            modelBuilder.Entity<TaskItem>()
                .HasQueryFilter(t => !t.IsDeleted);

            base.OnModelCreating(modelBuilder);
        }
    }
}
