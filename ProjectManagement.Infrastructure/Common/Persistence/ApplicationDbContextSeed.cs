using Microsoft.EntityFrameworkCore;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Infrastructure.Common.Persistence
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedSampleDataAsync(ApplicationDbContext context)
        {
            if (await context.Projects.AnyAsync()) return;

            var ownerId = Guid.NewGuid();

            // 1. Creamos los Proyectos
            var project1 = new Project("Sistema de Gestión API", "Pruebas de endpoints", ownerId);
            var project2 = new Project("Proyecto Archivado", "Para validar RN-01", ownerId);
            project2.Archive();

            context.Projects.AddRange(project1, project2);

            // 2. Creamos las Tareas vinculándolas por ID
            var tasks = new List<TaskItem>
        {
            new(
                project1.Id,
                "Configurar persistencia",
                "DbContext y Fluent API",
                TaskPriority.High,
                DateTime.UtcNow.AddDays(7)),

            new(
                project1.Id,
                "Validar Middleware",
                "Pruebas de excepciones",
                TaskPriority.Medium,
                DateTime.UtcNow.AddDays(2))
        };

            context.TaskItems.AddRange(tasks);

            await context.SaveChangesAsync();
        }
    }
}
