using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Cinecon
{
    public class JsonHelper
    {
        public static List<Movie> Movies = new List<Movie>();

        public static void LoadJson()
        {
            Movies = JsonConvert.DeserializeObject<List<Movie>>(File.ReadAllText("Assets/movies.json"));
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
}
