
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DKSKMaui.Backend.Models;

public class JobDiscription
{
    [Key] // This marks it as the primary key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // This makes it auto-increment
    public int Id { get; set; }
    public string description { get; set; } = string.Empty;
    public int sizeBedroom { get; set; }
    public int sizeBathroom { get; set; }
    public int price { get; set; }
}
