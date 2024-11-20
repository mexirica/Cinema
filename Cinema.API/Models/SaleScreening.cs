using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cinema.API.Models
{
	public class SaleScreening
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[ForeignKey(nameof(Sale))]
		public int SaleId { get; set; }

		[Required]
		[ForeignKey(nameof(Screening))]
		public int ScreeningId { get; set; }

		public Sale? Sale { get; set; }
		public Screening? Screening { get; set; }
		
		public bool UnassignedSeat { get; set; }
	}
}
