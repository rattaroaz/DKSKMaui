
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DKSKMaui.Backend.Models;

public class Supervisor
{
    [Key] // This marks it as the primary key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // This makes it auto-increment
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public int CompanyId { get; set; }

    // Navigation property to Companny
    [ForeignKey("CompanyId")]
    [Required]
    public Companny Company { get; set; } = null!; // Navigation property to Companny

    // Navigation Property for related properties
    public List<Properties> Properties { get; set; } = new List<Properties>(); // Supervisor can manage multiple properties
}

