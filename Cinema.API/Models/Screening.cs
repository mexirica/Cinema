using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cinema.API.Models
{
	public class Screening
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[ForeignKey(nameof(Movie))]
		public int MovieId { get; set; }

		[Required]
		public DateTime Date { get; set; }

		[Required]
		public TimeSpan Time { get; set; }

		[Required]
		[ForeignKey(nameof(Room))]
		public int RoomId { get; set; }

		[Required]
		[Column(TypeName = "decimal(10, 2)")]
		public decimal Price { get; set; }

		public Movie? Movie { get; set; }
		public Room? Room { get; set; }
	}

}