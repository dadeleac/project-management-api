using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Api.Models.Requests;
using ProjectManagement.Application.Common.Pagination;
using ProjectManagement.Application.Projects.Commands.ArchiveProject;
using ProjectManagement.Application.Projects.Commands.CreateProject;
using ProjectManagement.Application.Projects.Queries.GetProjects;
using ProjectManagement.Application.Projects.Queries.GetProjectsById;
using ProjectManagement.Application.TaskItems.Commands.CreateTaskItem;
using ProjectManagement.Application.TaskItems.Queries.GetTaskItems;

namespace ProjectManagement.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// Crea un nuevo proyecto.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateProjectRequest request, CancellationToken ct)
        {
            var command = new CreateProjectCommand(
                request.Name,
                request.Description,
                request.OwnerId);

            var projectId = await _mediator.Send(command, ct);

            return CreatedAtAction(nameof(GetById), new { id = projectId }, projectId);
        }

        /// <summary>
        /// Lista proyectos con filtros y paginación obligatoria (RN-06).
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<ProjectListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] GetProjectsRequest request, CancellationToken ct)
        {
            var query = new GetProjectsQuery(
                request.Status,
                request.PageNumber,
                request.PageSize);

            var result = await _mediator.Send(query, ct);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el detalle de un proyecto con su resumen estadístico (RN-05).
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ProjectDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetProjectByIdQuery(id), ct);
            return Ok(result);
        }

        /// <summary>
        /// Archiva un proyecto si no tiene tareas en curso (RN-02).
        /// </summary>
        [HttpPatch("{id:guid}/archive")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Archive(Guid id, CancellationToken ct)
        {
            await _mediator.Send(new ArchiveProjectCommand(id), ct);
            return NoContent();
        }
        
        /// <summary>
        /// Crear tarea asociada a un proyecto (RN-01).
        /// </summary>
        [HttpPost("{projectId:guid}/tasks")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateTask(Guid projectId, [FromBody] CreateTaskRequest request, CancellationToken ct)
        {
            var command = new CreateTaskItemCommand(
                projectId,
                request.Title,
                request.Description,
                request.Priority,
                request.DueDate);

            var taskId = await _mediator.Send(command, ct);
            
            return CreatedAtRoute("GetTaskById", new { id = taskId }, taskId);
        }

        /// <summary>
        /// Listar tareas de un proyecto — filtro por Status y Priority, paginado.
        /// </summary>
        [HttpGet("{projectId:guid}/tasks")]
        [ProducesResponseType(typeof(PagedResponse<TaskItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTasks(Guid projectId, [FromQuery] GetTasksRequest request, CancellationToken ct)
        {
            var query = new GetTasksQuery(
                projectId,
                request.Status,
                request.Priority,
                request.PageNumber,
                request.PageSize);

            var result = await _mediator.Send(query, ct);
            return Ok(result);
        }


    }
}
