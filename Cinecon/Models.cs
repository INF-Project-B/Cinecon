using Newtonsoft.Json;
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
        public bool IsTaken { get; set; } = true;
    }
}
