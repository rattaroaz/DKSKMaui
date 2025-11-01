
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DKSKMaui.Backend.Models;

public class Properties
{
    [Key] // This marks it as the primary key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // This makes it auto-increment
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
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

    public int SupervisorId { get; set; }

    // Navigation Property
    [ForeignKey("SupervisorId")]
    [JsonIgnore] // Prevents cycle during serialization
    [Required]
    public Supervisor Supervisor { get; set; } = null!; // Property is associated with a supervisor
}
