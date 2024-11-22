using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.API.Models;

public class Sale
{
    [Key] public int Id { get; set; }

    [Required]
    [ForeignKey(nameof(Customer))]
    public int CustomerId { get; set; }

    [Required] public DateTime SaleDate { get; set; }

    [Required]
    [Column(TypeName = "decimal(10, 2)")]
    public decimal AmountPaid { get; set; }

    // Navigation property
    public Customer? Customer { get; set; }
}