using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Application.Common.Exceptions
{
    public sealed record ValidationError(string Property, string Code, string Message);
}
