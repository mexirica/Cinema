using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cinema.API.Models
{
	public class SaleScreeningSeat
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[ForeignKey(nameof(SaleScreening))]
		public int SaleScreeningId { get; set; }

		[Required]
		[ForeignKey(nameof(Seat))]
		public int SeatId { get; set; }

		public SaleScreening? SaleScreening { get; set; }
		public Seat? Seat { get; set; }
	}
}
