﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace Cinecon
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string[] Genres { get; set; }
        public int Room { get; set; }
        // The key is the day. The value is an array of the times.
        public Dictionary<string, string[]> Days { get; set; }
        public bool BringId { get; set; }
    }

    public class MenuCategory
    {
        public string Name { get; set; }
        public List<MenuItem> MenuItems { get; set; }
    }

    public class MenuItem
    {
        public string Name { get; set; }
        // The key is the size/product type. The value is the price.
        public Dictionary<string, double> ItemTypes { get; set; }
    }

    public class Reservation
    {
        public string Code { get; set; }
        [JsonProperty("is_activated")]
        public bool IsActivated { get; set; }
        [JsonProperty("payment_method")]
        public string PaymentMethod { get; set; }
        public List<Seat> Seats { get; set; }
    }

    public class Seat
    {
        public string Row { get; set; }
        public int Number { get; set; }
        [JsonProperty("taken")]
        public bool IsTaken { get; set; } = true;
    }

    public class Room
    {
        public int Number { get; set; }
        [JsonProperty("total_seats")]
        public int TotalSeats { get => TotalRows * SeatsPerRow; }
        [JsonProperty("total_rows")]
        public int TotalRows { get; set; }
        [JsonProperty("seats_per_row")]
        public int SeatsPerRow { get; set; }
        public List<Movie> Movies { get; set; }
        public List<Seat> Seats
        {
            get 
            {
                var seats = new List<Seat>();
                for (int i = 0; i < TotalRows; i++)
                    for (int j = 0; j < SeatsPerRow; j++)
                        seats.Add(new Seat { Row = ((char)(65 + i)).ToString(), Number = j + 1, IsTaken = false });
                return seats;
            } 
        }
    }
}
