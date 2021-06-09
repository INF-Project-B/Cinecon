using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinecon
{
    public static class MovieSystem
    {
        private static List<KeyValuePair<string, Action>> _genres;

        public static void ShowFilteredFilms()
        {
            ConsoleHelper.LogoType = LogoType.Films;
            ConsoleHelper.Breadcrumb = $"Genres: {(_genres?.Count > 0 ? string.Join(", ", _genres.Select(x => x.Key)) : "Alle")}\n";

            var movies = new Dictionary<string, Action>();

            foreach (var movie in JsonHelper.Movies)
            {
                if (_genres?.Count > 0 && movie.Genres.Intersect(_genres.Select(x => x.Key)).Count() == 0)
                    continue;
                movies[movie.Title] = null;
            }

            var movieMenu = new ChoiceMenu(movies, true);

            var movieChoice = movieMenu.MakeChoice();

            movieChoice.Value?.Invoke();

            if (movieChoice.Key == "Terug")
                ShowFilms();
            else if (movieChoice.Value == null)
                ShowFilmInfo(movieChoice.Key);
        }
       


        public static void ListOfFilms()
        {
            ConsoleHelper.LogoType = LogoType.Films;
            ConsoleHelper.Breadcrumb = $"Films / Alle films";

            var listofmovies = new Dictionary<string, Action>();
        
            foreach (var movie in JsonHelper.Movies)
                listofmovies[movie.Title] = null;

            var movieChoiceList = new ChoiceMenu(listofmovies, true);
            var filmChoice = movieChoiceList.MakeChoice();
            if (filmChoice.Key == "Terug")
                ShowFilms();
            else
                ShowFilmInfo(filmChoice.Key);
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

            foreach (var date in JsonHelper.Days.Where(x => x.Item2.Any(x => x.Movies.Any(x => x.Id == movie.Id))))
            {
                var day = $"{ConsoleHelper.TranslateDate(date.Item1.ToString("dddd"))} {date.Item1:dd} {ConsoleHelper.TranslateDate(date.Item1.ToString("MMMM"))}";
                listOfDays[day] = null;
            }

            var msg = listOfDays.Count > 0 ? "" : "Geen dagen gevonden.";
            var dayChoiceMenu = new ChoiceMenu(listOfDays, true, msg);
            var dayChoice = dayChoiceMenu.MakeChoice();

            if (dayChoice.Key == "Terug")
                ShowDateAndTime(movie);
            else
                ChooseFilmTime(movie, dayChoice.Key);
        }

        private static void ChooseFilmTime(Movie movie, string day)
        {
            ConsoleHelper.LogoType = LogoType.Films;
            ConsoleHelper.Breadcrumb = $"Films / {movie.Title} / Dagen & Tijden";

            var listOfTimes = new Dictionary<string, Action>();
            foreach (var time in movie.Times)
                listOfTimes[time] = null;

            var timeChoiceMenu = new ChoiceMenu(listOfTimes, true, listOfTimes.Count > 0 ? "" : "Geen tijden gevonden.");

            var timeChoice = timeChoiceMenu.MakeChoice();

            if (timeChoice.Key == "Terug")
                ChooseFilmDays(movie);
            else
                ReservationSystem.ChooseTicketsAmount(movie, JsonHelper.Days.FirstOrDefault(x => $"{ConsoleHelper.TranslateDate(x.Item1.ToString("dddd"))} {x.Item1:dd} {ConsoleHelper.TranslateDate(x.Item1.ToString("MMMM"))}" == day).Item1, timeChoice.Key);
        }        



        public static void ShowFilms()
        {
            ConsoleHelper.LogoType = LogoType.Films;

            ConsoleHelper.Breadcrumb = $"Genres: {(_genres?.Count > 0 ? string.Join(", ", _genres.Select(x => x.Key)) : "Alle")}\n";

            Dictionary<string, Action> movies = new Dictionary<string, Action>
            {
                    { "Filter op genres", ShowGenresFilter },
                    { "Filter op dagen", ShowDaysFilter }
            };
            if (_genres == null)
                movies["Bekijk alle films"] = ListOfFilms;

            else
            {
                movies["Reset filters"] = () => { _genres = null; };
                movies["Bekijk alle films"] = ListOfFilms;
            }
            var movieMenu = new ChoiceMenu(movies, true);

            var movieChoice = movieMenu.MakeChoice();

            movieChoice.Value?.Invoke();

            if (movieChoice.Key == "Terug")
                Program.StartChoice();
            else if (movieChoice.Key == "Reset filters")
                ShowFilms();
        }

        private static void ShowDaysFilter() 
        {
            ConsoleHelper.LogoType = LogoType.Films;
            ConsoleHelper.Breadcrumb = "Films / Filters / Dagen";

            var dayOptions = new Dictionary<string, Action>();

            foreach (var date in JsonHelper.Days) 
            {
                var day = $"{ConsoleHelper.TranslateDate(date.Item1.ToString("dddd"))} {date.Item1:dd} {ConsoleHelper.TranslateDate(date.Item1.ToString("MMMM"))}";
                dayOptions[day] = null;
            }

            var dayChoiceMenu = new ChoiceMenu(dayOptions, true);

            var dayChoice = dayChoiceMenu.MakeChoice();

            if (dayChoice.Key == "Terug")
                ShowFilms();
            else
                ShowTimes(JsonHelper.Days.FirstOrDefault(x => $"{ConsoleHelper.TranslateDate(x.Item1.ToString("dddd"))} {x.Item1:dd} {ConsoleHelper.TranslateDate(x.Item1.ToString("MMMM"))}" == dayChoice.Key).Item1);

            static void ShowTimes(DateTime date)
            {
                ConsoleHelper.Breadcrumb += $" / {date.DayOfWeek}";

                var filmTimes = new Dictionary<string, Action>();

                foreach (var room in JsonHelper.Days.FirstOrDefault(x => x.Item1 == date).Item2)
                    foreach (var movie in room.Movies)
                        foreach (var time in movie.Times)
                            filmTimes[$"{time} {movie.Title}"] = null;

                var text = filmTimes.Count > 0 ? "" : "   Geen tijden gevonden.";

                var filmTimeMenu = new ChoiceMenu(filmTimes, true, text);

                var filmTimeChoice = filmTimeMenu.MakeChoice();

                if (filmTimeChoice.Key == "Terug")
                    ShowDaysFilter();
                else
                    ReservationSystem.ChooseTicketsAmount(JsonHelper.Movies.FirstOrDefault(x => x.Title == filmTimeChoice.Key[12..]), date, filmTimeChoice.Key[0..11]);
                
            }
        }

        private static void ShowGenresFilter()
        {
            ConsoleHelper.LogoType = LogoType.Films;
            ConsoleHelper.Breadcrumb = "Films / Filters / Genres";

            var genreChoices = new Dictionary<string, Action>();

            foreach (var genre in JsonHelper.Genres)
                genreChoices[genre] = null;

            var genreChoiceMenu = new ChoiceMenu(genreChoices, addExtraButton: true);
            
            _genres = genreChoiceMenu.MakeMultipleChoice(_genres);

            if (_genres.Count() == 0)
                ShowFilms();
            else
                ShowFilteredFilms();
        }
    }
}
