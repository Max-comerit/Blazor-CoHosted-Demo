using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Shared.DTOs;
public record CompanyCreateDto : CompanyForManipulationDto
{
    public IEnumerable<EmployeeDto>? Employees { get; set; }
}
