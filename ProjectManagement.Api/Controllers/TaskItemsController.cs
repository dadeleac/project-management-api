using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Api.Models.Requests;
using ProjectManagement.Application.TaskItems.Commands.DeleteTaskItem;
using ProjectManagement.Application.TaskItems.Commands.UpdateTaskItemStatus;
using ProjectManagement.Application.TaskItems.Queries.GetTaskById;
using ProjectManagement.Application.TaskItems.Queries.GetTaskItems;

namespace ProjectManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskItemsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// Detalle de una tarea específica.
        /// </summary>
        [HttpGet("{id:guid}", Name = "GetTaskById")]
        [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetTaskByIdQuery(id), ct);
            return Ok(result);
        }

        /// <summary>
        /// Cambiar el Status de una tarea (RN-03).
        /// </summary>
        [HttpPatch("{id:guid}/status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateTaskStatusRequest request, CancellationToken ct)
        {

            var command = new UpdateTaskItemStatusCommand(id, request.NewStatus);

            await _mediator.Send(command, ct);
            return NoContent();
        }

        /// <summary>
        /// Eliminar tarea (soft delete — IsDeleted = true) (RN-04).
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            await _mediator.Send(new DeleteTaskItemCommand(id), ct);
            return NoContent();
        }

    }
}
