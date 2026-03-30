using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Constraints;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Persistence.Configurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable(TableName.Projects);

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .HasMaxLength(DomainConstraints.ProjectNameMaxLength)
                .IsRequired();

            builder.Property(p => p.Description)
                .HasMaxLength(500);

            builder.Property(p => p.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(p => p.CreatedAt)
                .IsRequired();
        }
    }
}
