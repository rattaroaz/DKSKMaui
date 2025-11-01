using System.ComponentModel.DataAnnotations;

// DTO for the Property entity
namespace DKSKMaui.Backend.Dto;

public class CompannyDto
{
    [Required(ErrorMessage = "Company name is required")]
    [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters")]
    public string Name { get; set; }
    
    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string? Phone { get; set; }
    
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public string? Email { get; set; }
    
    [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
    public string? Address { get; set; }
    
    [StringLength(50, ErrorMessage = "City cannot exceed 50 characters")]
    public string? City { get; set; }
    
    [StringLength(10, ErrorMessage = "Zip code cannot exceed 10 characters")]
    public string? Zip { get; set; }
    
    [StringLength(500, ErrorMessage = "Special note cannot exceed 500 characters")]
    public string? SpecialNote { get; set; }
    public List<SupervisorDto> Supervisors { get; set; } = new List<SupervisorDto>();


}
