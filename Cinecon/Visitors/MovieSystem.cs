using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cinecon
{
    public static class MovieSystem
    {
        private static List<KeyValuePair<string, Action>> _genres;
        private static KeyValuePair<string, string[]> _dayAndTimes;

        public static void ShowFilms()
        {
            ConsoleHelper.LogoType = LogoType.Films;
            ConsoleHelper.Breadcrumb = $"Genres: {(_genres?.Count > 0 ? string.Join(", ", _genres.Select(x => x.Key)) : "Alle")}\n" +
                $"   Tijden: {(_dayAndTimes.Key != null && _dayAndTimes.Value.Length > 0 ? $"{_dayAndTimes.Key} om {string.Join(", ", _dayAndTimes.Value)}" : "Alle") }";

            var movies = new Dictionary<string, Action>
            {
                {"Zoek films op filters" , ShowFilters },
                {"Bekijk alle films", ListOfFilms }
            };

            // r.44

            var movieMenu = new ChoiceMenu(movies, true);

            var movieChoice = movieMenu.MakeChoice();

            movieChoice.Value?.Invoke();

            if (movieChoice.Key == "Terug")
                Program.StartChoice();
            else if (movieChoice.Value == null)
                ShowFilmInfo(movieChoice.Key);
        }

        public static void ShowFilmInfo(string movieName)
        {
            ConsoleHelper.LogoType = LogoType.Films;
            ConsoleHelper.Breadcrumb = $"Films / {movieName}";
            
            var movie = JsonHelper.Movies.FirstOrDefault(x => x.Title == movieName);

            var text = $"   Titel: {movie.Title}\n\n   Omschrijving: { movie.Description}\n\n   Zaal: {movie.Room}\n\n_______________________________________\n";       

            var filmChoiceMenu = new ChoiceMenu(new Dictionary<string, Action>
            {
                { "Koop tickets", null }
            }, addBackChoice: true, text);

            var filmChoice = filmChoiceMenu.MakeChoice();

            if (filmChoice.Key == "Terug")
                ShowFilms();
            else
                ShowDateAndTime(movie);
        }
        
        private static void ShowDateAndTime(Movie movie)
        {
            ConsoleHelper.LogoType = LogoType.Films;
            ConsoleHelper.Breadcrumb = $"Films / {movie.Title}";

            var dateMenu = new ChoiceMenu(new Dictionary<string, Action>
            {
                {"Dag en tijden", null},
            }, addBackChoice: true);

            var dateChoice = dateMenu.MakeChoice();

            if (dateChoice.Key == "Dag en tijden")
                ChooseFilmDays(movie);
            else
                ShowFilmInfo(movie.Title);
        }

        private static void ChooseFilmDays(Movie movie)
        {
            ConsoleHelper.LogoType = LogoType.Films;
            ConsoleHelper.Breadcrumb = $"Films / {movie.Title} / Dagen & Tijden";

            var listOfDays = new Dictionary<string,Action>();

            foreach (var day in movie.Days.Where(x => x.Value.Any()))
                listOfDays[day.Key.First().ToString().ToUpper() + day.Key.Substring(1)] = null;

            var msg = listOfDays.Count > 0 ? "" : "Geen dagen gevonden.";
            var dayChoiceMenu = new ChoiceMenu(listOfDays, true, msg);
            var dayChoice = dayChoiceMenu.MakeChoice();

            if (dayChoice.Key == "Terug")
                ShowDateAndTime(movie);
            else
                ChooseFilmTime(movie, dayChoice.Key.ToLower());
        }

        private static void ChooseFilmTime(Movie movie, string day)
        {
            ConsoleHelper.LogoType = LogoType.Films;
            ConsoleHelper.Breadcrumb = $"Films / {movie.Title} / Dagen & Tijden";

            var listOfTimes = new Dictionary<string, Action>();
            foreach (var time in movie.Days[day])
                listOfTimes[time] = null;

            var timeChoiceMenu = new ChoiceMenu(listOfTimes, true, listOfTimes.Count > 0 ? "" : "Geen tijden gevonden.");

            var dayAndTime = new Dictionary<string, Action>();
            if (_dayAndTimes.Value != null && _dayAndTimes.Key == day)
            {
                foreach (var time in _dayAndTimes.Value)
                    dayAndTime[time] = null;
            }

            var dateChoice = timeChoiceMenu.MakeChoice();

            if (dateChoice.Key == "Terug")
                ChooseFilmDays(movie);
            else
                ReservationSystem.ChooseTicketsAmount(movie, day.ToString(), dateChoice.Key.ToString()); 
        }        

        private static void ShowFilters()
        {
            ConsoleHelper.LogoType = LogoType.Films;
            ConsoleHelper.Breadcrumb = "Films / Filters";

            var filtersChoiceMenu = new ChoiceMenu(new Dictionary<string, Action>
            {
                { "Genres", ShowGenresFilter },
                { "Dag en tijden", ShowDaysFilter },
                { "Reset filters", () => { _genres = null; _dayAndTimes = new KeyValuePair<string, string[]>(); } },
            }, addBackChoice: true);

            var filtersChoice = filtersChoiceMenu.MakeChoice();

            filtersChoice.Value?.Invoke();

            if (filtersChoice.Key == "Terug")
                ShowFilms();
            else if (filtersChoice.Key == "Reset filters")
                ShowFilters();
        }

        private static void ShowDaysFilter()
        {
            ConsoleHelper.LogoType = LogoType.Films;
            ConsoleHelper.Breadcrumb = "Films / Filters / Dag en tijden";

            var dayOptions = new Dictionary<string, Action>();
            
            foreach (var day in new[] { "Maandag", "Dinsdag", "Woensdag", "Donderdag", "Vrijdag", "Zaterdag", "Zondag" })
                dayOptions[day] = null;

            var dayChoiceMenu = new ChoiceMenu(dayOptions, true);

            var dayChoice = dayChoiceMenu.MakeChoice(_dayAndTimes.Key != null ? new[] { _dayAndTimes.Key } : null);

            if (dayChoice.Key == "Terug")
                ShowFilters();
            else
                ShowTimes(dayChoice.Key);

            static void ShowTimes(string day)
            {
                ConsoleHelper.Breadcrumb += $" / {day}";

                var timeOptions = new Dictionary<string, Action>();

                foreach (var movie in JsonHelper.Movies)
                    foreach (var time in movie.Days[day.ToLower()])
                        timeOptions[time] = null;

                var text = timeOptions.Count > 0 ? "" : "   Geen tijden gevonden.";

                var timeChoiceMenu = new ChoiceMenu(timeOptions, true, text);

                var dayAndTimes = new Dictionary<string, Action>();
                if (_dayAndTimes.Value != null && _dayAndTimes.Key == day)
                {
                    foreach (var time in _dayAndTimes.Value)
                        dayAndTimes[time] = null;
                }

                var timeChoices = timeChoiceMenu.MakeMultipleChoice(dayAndTimes.ToList());

                if (timeChoices.Count > 0 || _dayAndTimes.Key == day)
                    _dayAndTimes = new KeyValuePair<string, string[]>(day, timeChoices.Select(x => x.Key).ToArray());

                ShowDaysFilter();
            }
        }

        private static void ShowGenresFilter()
        {
            ConsoleHelper.LogoType = LogoType.Films;
            ConsoleHelper.Breadcrumb = "Films / Filters / Genres";

            var genreChoices = new Dictionary<string, Action>();

            foreach (var genre in JsonHelper.Genres)
                genreChoices[genre] = null;

            var genreChoiceMenu = new ChoiceMenu(genreChoices, true);

            _genres = genreChoiceMenu.MakeMultipleChoice(_genres);

            ShowFilters();
        }
    }
}
