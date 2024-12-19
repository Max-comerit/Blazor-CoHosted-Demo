namespace TestWithBasicAuth.Models
{
    public class CompanyDto
    {
            public int Id { get; set; }  // Temporär lösning om id är string i JSON
            public string Name { get; set; }
            public string Address{ get; set; }
            public List<EmployeeDto> Employees { get; set; }
        }
}
