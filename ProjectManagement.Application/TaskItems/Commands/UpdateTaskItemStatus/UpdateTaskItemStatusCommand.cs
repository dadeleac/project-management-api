using MediatR;
using ProjectManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Application.TaskItems.Commands.UpdateTaskItemStatus
{
    public sealed record UpdateTaskItemStatusCommand(
        Guid TaskItemId, 
        TaskItemStatus NewStatus) : IRequest;
}
