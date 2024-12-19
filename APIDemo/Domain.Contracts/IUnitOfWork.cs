namespace Domain.Contracts;

public interface IUnitOfWork
{
    ICompanyRepository CompanyRepository { get; }
    IEmployeeRepository EmployeeRepository { get; }

    Task CompleteASync();
}