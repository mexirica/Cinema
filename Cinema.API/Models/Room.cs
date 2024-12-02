using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Cinema.API.Models;

public class Room
{
    [Key] public int Id { get; set; }

    [Required] public string Name { get; set; } = string.Empty;

    [JsonIgnore]public ICollection<RoomSeat> RoomSeat { get; set; } = new List<RoomSeat>();
}