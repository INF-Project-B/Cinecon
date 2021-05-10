using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Cinecon
{
    public static class JsonHelper
    {
        public static List<Movie> Movies { get; set; }
        public static List<string> Genres { get; set; }
        public static List<MenuCategory> Menu { get; set; }
        public static List<Reservation> Reservations { get; set; }
        public static List<Room> Rooms { get; set; }

        public static void LoadJson()
        {
            AddMovies();
            AddGenres();
            AddMenu();
            AddRooms();
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
            => Reservations = JsonConvert.DeserializeObject<List<Reservation>>(File.ReadAllText("Assets/reservations.json"));

        private static void AddRooms()
            => Rooms = JsonConvert.DeserializeObject<List<Room>>(File.ReadAllText("Assets/rooms.json"));

        public static void UpdateJsonFiles()
        {
            var settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

            File.WriteAllText("Assets/movies.json", JsonConvert.SerializeObject(Movies, Formatting.Indented, settings));
            File.WriteAllText("Assets/reservations.json", JsonConvert.SerializeObject(Reservations, Formatting.Indented, settings));
            File.WriteAllText("Assets/rooms.json", JsonConvert.SerializeObject(Rooms, Formatting.Indented, settings));

            var menuJson = new JObject();
            foreach (var menuCategory in Menu)
            {
                var menuCategoryJson = new JObject();
                foreach (var menuItem in menuCategory.MenuItems)
                    menuCategoryJson.Add(menuItem.Name, JToken.Parse(JsonConvert.SerializeObject(menuItem.ItemTypes)));

                menuJson.Add(menuCategory.Name, menuCategoryJson);
            }

            File.WriteAllText("Assets/menu.json", JsonConvert.SerializeObject(menuJson, Formatting.Indented, settings));
        }
    }
}
