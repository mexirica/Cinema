using System.ComponentModel.DataAnnotations;

namespace Cinema.API.Models
{
	public class Seat
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Number { get; set; } = string.Empty;

		[Required]
		public string Row { get; set; } = string.Empty;
	}
}
