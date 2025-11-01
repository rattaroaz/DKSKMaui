
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DKSKMaui.Backend.Models;

public class Invoice
{
    [Key] // This marks it as the primary key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // This makes it auto-increment
    public int Id { get; set; }
    public DateTime TodaysDate { get; set; }
    public DateTime WorkDate { get; set; }

    public string CompanyName { get; set; } = string.Empty;
    public string PropertyAddress { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string? GateCode { get; set; }
    public string? LockBox { get; set; }
    public int SizeBedroom {  get; set; }
    public int SizeBathroom { get; set; }
    public string? WorkOrder { get; set; }
    public string JobDescriptionChoice { get; set; } = string.Empty;
    public string? ContractorName { get; set; }
    public int AmountCost { get; set; }
    public int AmountPaid1 { get; set; }
    public DateTime DatePaid1 { get; set; }
    public string? CheckNumber1 { get; set; }
    public int AmountPaid2 { get; set; }
    public DateTime DatePaid2 { get; set; }
    public string? CheckNumber2 { get; set; }

    public DateTime? InvoiceCreatedDate { get; set; }
    public string? SpecialNote { get; set; }
    public string? GarageRemoteCode { get; set; }
    public int Status   { get; set; }

}

public class Payment
{
    public DateTime Date { get; set; }
    public int Amount { get; set; }
    public string CheckNumber { get; set; } = string.Empty;
}
