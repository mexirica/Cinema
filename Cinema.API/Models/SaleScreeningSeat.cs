using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cinema.API.Models;

public class SaleScreeningSeat
{
    [Required]
    [ForeignKey(nameof(SaleScreening))]
    public int SaleScreeningId { get; set; }

    [Required] [ForeignKey(nameof(Seat))] public int SeatId { get; set; }

    [JsonIgnore]public SaleScreening? SaleScreening { get; set; }
    [JsonIgnore]public Seat? Seat { get; set; }
}