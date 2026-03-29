using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Interfaces.Commands;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Infrastructure.Common.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Infrastructure.Repositories.Commands
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Project?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _context.Projects.FindAsync(new object[] { id }, ct);
        }

        public async Task AddAsync(Project project, CancellationToken ct)
        {
            await _context.Projects.AddAsync(project, ct);
        }

        public async Task UpdateAsync(Project project, CancellationToken ct)
        {
            _context.Projects.Update(project);
        }

        public async Task SaveChangesAsync(CancellationToken ct)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}
