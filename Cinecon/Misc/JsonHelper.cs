using System;
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
        public static ReservationData ReservationData { get; set; }
        public static List<Tuple<DateTime, List<Room>>> Days { get; set; }
        public static EmailData EmailData { get; set; }

        public static void LoadJson()
        {
            AddMovies();
            AddGenres();
            AddMenu();
            AddDays();
            AddReservations();
            AddEmailData();

            UpdateJsonFiles();
        }

        private static void AddDays()
        {
            var daysJson = (JArray)JsonConvert.DeserializeObject(File.ReadAllText("Assets/days.json"));

            var days = new List<Tuple<DateTime, List<Room>>>();

            var today = DateTime.Now;
            for (int i = 0; i < 15; i++)
            {
                var date = today.AddDays(i);
                var rooms = daysJson[0]["item2"].ToObject<List<Room>>();

                foreach (var room in rooms)
                    room.Date = date;

                days.Add(Tuple.Create(date, rooms));
            }

            Days = days;
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
                        ItemTypes = menuItem.Value.ToObject<Dictionary<string, decimal>>()
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
            var reservationData = JsonConvert.DeserializeObject<ReservationData>(File.ReadAllText("Assets/reservations.json"));
            foreach (var reservation in reservationData.Reservations)
                reservation.Date = DateTime.Now;
            ReservationData = reservationData;
        }

        private static void AddEmailData()
            => EmailData = JsonConvert.DeserializeObject<EmailData>(File.ReadAllText("Assets/email.json"));

        public static void UpdateJsonFiles()
        {
            var settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

            File.WriteAllText("Assets/movies.json", JsonConvert.SerializeObject(Movies, Formatting.Indented, settings));
            File.WriteAllText("Assets/reservations.json", JsonConvert.SerializeObject(ReservationData, Formatting.Indented, settings));
            File.WriteAllText("Assets/days.json", JsonConvert.SerializeObject(Days, Formatting.Indented, settings));

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
