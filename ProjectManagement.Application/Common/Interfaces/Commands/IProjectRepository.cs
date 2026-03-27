using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Common.Interfaces.Commands
{
    public interface IProjectRepository
    {
        Task<Project?> GetByIdAsync(Guid id, CancellationToken ct);
        Task SaveAsync(Project project, CancellationToken ct);
    }
}
