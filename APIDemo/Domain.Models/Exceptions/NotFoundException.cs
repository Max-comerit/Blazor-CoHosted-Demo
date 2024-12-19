using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Exceptions;
public abstract class NotFoundException : Exception
{
    public string Title { get; }
    protected NotFoundException(string message, string title = "Not Found") : base(message)
    {
        Title = title;
    }

}

public class CompanyNotFoundException : NotFoundException
{
    public CompanyNotFoundException(int id) : base($"The comapany with {id} is not found")
    {
        
    }
}

public class EmployeeNotFoundException : NotFoundException
{
    public EmployeeNotFoundException(int id) : base($"The emplyee with {id} is not found")
    {

    }
}
