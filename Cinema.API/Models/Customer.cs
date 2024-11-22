using System.ComponentModel.DataAnnotations;

namespace Cinema.API.Models;

public class Customer
{
    [Key] public int Id { get; set; }

    [Required] public string Name { get; set; } = string.Empty;

    [Required] [EmailAddress] public string Email { get; set; } = string.Empty;
}