using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Common.Interfaces.Commands
{
    public interface IProjectRepository
    {
        Task<Project?> GetByIdAsync(Guid id, CancellationToken ct);
        Task AddAsync(Project project, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
