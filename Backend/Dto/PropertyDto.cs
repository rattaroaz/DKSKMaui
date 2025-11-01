// DTO for the Property entity
namespace DKSKMaui.Backend.Dto;

public class PropertyDto
{
    public string Name { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Zip { get; set; }
    public string? GateCode { get; set; }
    public string? GarageRemoteCode { get; set; }
    public string? ManagerName { get; set; }
    public string? ManagerPhone { get; set; }
    public string? ManagerEmail { get; set; }

    public string? LockBox { get; set; }
    public string? SpecialNote { get; set; }
    public bool? IsActive { get; set; }


}
