
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DKSKMaui.Backend.Models;

public class Companny
{
    [Key] // This marks it as the primary key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // This makes it auto-increment
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Company name is required")]
    [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Owner name cannot exceed 100 characters")]
    public string? Owner { get; set; }
    
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
    public string? Zip {  get; set; }
    
    [StringLength(500, ErrorMessage = "Special note cannot exceed 500 characters")]
    public string? SpecialNote { get; set; }

    // Navigation property
    public List<Supervisor> Supervisors { get; set; } = new List<Supervisor>();

}
