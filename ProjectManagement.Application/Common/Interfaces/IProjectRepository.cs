using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Common.Interfaces
{
    public interface IProjectRepository
    {
        Task CreateAsync(Project project, CancellationToken ct);
        Task<Project?> GetByIdAsync(Guid id, CancellationToken ct);
        Task SaveAsync(Project project, CancellationToken ct);
    }
}
