using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Application.Projects.Queries.GetProjectsById
{
    public sealed record ProjectSummaryDto(
        int TotalTasks,
        int CompletedTasks,
        int InProgressTasks,
        int PendingTasks);
}
