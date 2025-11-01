
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DKSKMaui.Backend.Models;

public class Contractor
{
    [Key] // This marks it as the primary key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // This makes it auto-increment
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Contractor name is required")]
    [StringLength(100, ErrorMessage = "Contractor name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(50, ErrorMessage = "License number cannot exceed 50 characters")]
    public string? LicenseNumber { get; set; }
    
    [StringLength(20, ErrorMessage = "Social Security Number cannot exceed 20 characters")]
    public string? SocailSecurityNumber { get; set; }
    
    [StringLength(50, ErrorMessage = "Contractor ID cannot exceed 50 characters")]
    public string? ContractorID { get; set; }
    
    [StringLength(10, ErrorMessage = "Payroll percent cannot exceed 10 characters")]
    public string? PayrollPercent { get; set; }
    
    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(20, ErrorMessage = "Cell phone cannot exceed 20 characters")]
    public string? CellPhone { get; set; }
    
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
    
    public bool? IsActive { get; set; }

}
