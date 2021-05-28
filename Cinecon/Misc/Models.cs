using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public Dictionary<string, decimal> ItemTypes { get; set; }
    }

    public class ReservationData
    {
        [JsonProperty("payment_methods")]
        public string[] PaymentMethods { get; set; }
        public List<Reservation> Reservations { get; set; }
    }

    public class Reservation
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [JsonProperty("is_activated")]
        public bool IsActivated { get; set; }
        [JsonProperty("payment_method")]
        public string PaymentMethod { get; set; }
        public int Room { get; set; }
        public DateTime Date { get; set; }
        public Movie Movie { get; set; }
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
        public List<Seat> Seats { get => GetSeats(); }

        public List<Seat> GetSeats(DateTime date = default)
        {
            var seats = new List<Seat>();
            for (int i = 0; i < TotalRows; i++)
                for (int j = 0; j < SeatsPerRow; j++)
                    seats.Add(new Seat { Row = ((char)(65 + i)).ToString(), Number = j + 1, IsTaken = JsonHelper.ReservationData?.Reservations?.Where(x => (date == default || x.Date.Date == date.Date) && x.Room == Number).Any(x => x.Seats.Any(x => x.Row == ((char)(65 + i)).ToString() && x.Number == j + 1)) ?? false });
            return seats;
        }
    }

    public class EmailData
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
