using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cinema.API.Models;

public class RoomSeat
{
    [Key] public int Id { get; set; }

    [Required] [ForeignKey(nameof(Room))] public int RoomId { get; set; }

    [Required] [ForeignKey(nameof(Seat))] public int SeatId { get; set; }

   [JsonIgnore] public Room? Room { get; set; }
    [JsonIgnore]public Seat? Seat { get; set; }
}