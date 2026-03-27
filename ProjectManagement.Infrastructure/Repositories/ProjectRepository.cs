using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Infrastructure.Common.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Infrastructure.Repositories
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

        public async Task SaveAsync(Project project, CancellationToken ct)
        {
            var entry = _context.Entry(project);

            if(entry.State == EntityState.Detached)
            {
                await _context.Projects.AddAsync(project, ct);
            }

            await _context.SaveChangesAsync(ct);
        }
    }
}
