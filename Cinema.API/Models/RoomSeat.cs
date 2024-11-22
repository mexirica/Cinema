﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.API.Models;

public class RoomSeat
{
    [Key] public int Id { get; set; }

    [Required] [ForeignKey(nameof(Room))] public int RoomId { get; set; }

    [Required] [ForeignKey(nameof(Seat))] public int SeatId { get; set; }

    public Room? Room { get; set; }
    public Seat? Seat { get; set; }
}