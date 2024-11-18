using System.ComponentModel.DataAnnotations;

namespace Cinema.API.Models
{
	public class Room
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Name { get; set; } = string.Empty;

		public ICollection<RoomSeat> RoomSeat { get; set; } = new List<RoomSeat>();
	}
}
