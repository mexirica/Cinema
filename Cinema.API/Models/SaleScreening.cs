using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cinema.API.Models;

public class SaleScreening
{
    [Key] public int Id { get; set; }

    [Required] [ForeignKey(nameof(Sale))] public int SaleId { get; set; }

    [Required]
    [ForeignKey(nameof(Screening))]
    public int ScreeningId { get; set; }

   [JsonIgnore] public Sale? Sale { get; set; }
    [JsonIgnore]public Screening? Screening { get; set; }

    public bool UnassignedSeat { get; set; }
}