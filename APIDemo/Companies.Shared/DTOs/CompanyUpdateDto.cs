namespace Companies.Shared.DTOs;

public record CompanyUpdateDto : CompanyForManipulationDto
{
    public int Id { get; set; }
}
