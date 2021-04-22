using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cinecon
{
    public static class JsonHelper
    {
        public static List<Movie> Movies { get; set; }
        public static List<string> Genres { get; set; }
        public static List<MenuCategory> Menu { get; set; }
        public static List<Reservation> Reservations { get; set; }

        public static void LoadJson()
        {
            AddMovies();
            AddGenres();
            AddMenu();
            AddReservations();
        }

        private static void AddMovies()
            => Movies = JsonConvert.DeserializeObject<List<Movie>>(File.ReadAllText("Assets/movies.json"));

        private static void AddGenres()
        {
            var genres = new List<string>();

            foreach (var genreList in Movies.Select(x => x.Genres))
                foreach (var genre in genreList)
                    genres.Add(genre);

            Genres = genres.Distinct().ToList();
        }

        private static void AddMenu()
        {
            var menuCategoriesJson = (JObject)JsonConvert.DeserializeObject(File.ReadAllText("Assets/menu.json"));

            var menu = new List<MenuCategory>();

            foreach (var menuCategory in menuCategoriesJson)
            {
                var menuItems = new List<MenuItem>();

                foreach (var menuItem in menuCategory.Value as JObject)
                {
                    menuItems.Add(new MenuItem
                    {
                        Name = menuItem.Key,
                        ItemTypes = menuItem.Value.ToObject<Dictionary<string, double>>()
                    });
                }

                menu.Add(new MenuCategory
                {
                    Name = menuCategory.Key,
                    MenuItems = menuItems
                });
            }

            Menu = menu;
        }

        private static void AddReservations()
        {
            Reservations = JsonConvert.DeserializeObject<List<Reservation>>(File.ReadAllText("Assets/reservation.json"));
        }
    }    
    
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string[] Genres { get; set; }
        public int Room { get; set; }
        public string[] Hours { get; set; }
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
