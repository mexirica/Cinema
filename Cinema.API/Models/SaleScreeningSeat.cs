using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.API.Models;

public class SaleScreeningSeat
{
    [Required]
    [ForeignKey(nameof(SaleScreening))]
    public int SaleScreeningId { get; set; }

    [Required] [ForeignKey(nameof(Seat))] public int SeatId { get; set; }

    public SaleScreening? SaleScreening { get; set; }
    public Seat? Seat { get; set; }
}