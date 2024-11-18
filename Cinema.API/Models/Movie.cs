using System.ComponentModel.DataAnnotations;

namespace Cinema.API.Models
{
	public class Movie
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Title { get; set; } = string.Empty;

		[Required]
		public float Rating { get; set; }

		public string? Description { get; set; }
		public string? ImageUrl { get; set; }
		public TimeSpan Duration { get; set; }
	}
}
