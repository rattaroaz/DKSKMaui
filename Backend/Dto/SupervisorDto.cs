// DTO for the Property entity
namespace DKSKMaui.Backend.Dto;

public class SupervisorDto
{
    public string Name { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public List<PropertyDto> Properties { get; set; } = new List<PropertyDto>();

}
