using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinecon
{
    public static class VisitorsMenu
    {
        private static List<KeyValuePair<string, Action>> _genres;
        private static KeyValuePair<string, string[]> _dayAndTimes;

        private static readonly List<KeyValuePair<string, decimal>> _menuCart = new List<KeyValuePair<string, decimal>>();
        private static string MenuCartText 
        { 
            get
            {
                var menuCart = new List<KeyValuePair<string, decimal>>();
                foreach (var item in _menuCart)
                {
                    var count = _menuCart.Count(x => x.Key == item.Key);
                    if (count > 1)
                        menuCart.Add(new KeyValuePair<string, decimal>(item.Key + $" (x{count})", item.Value));
                    else
                        menuCart.Add(item);
                }
                return $"   Winkelmand (totaal: {_menuCart.Select(x => x.Value).Sum():0.00} euro)\n     {(menuCart.Any() ? string.Join("\n     ", menuCart.ToHashSet().Select(x => x.Key)) : "Leeg")}\n";
            }
        }

        public static void ShowVisitorMenu()
        {
            ConsoleHelper.LogoType = LogoType.Visitor;
            ConsoleHelper.Breadcrumb = null;

            var visitorsMenu = new ChoiceMenu(new Dictionary<string, Action>
            {
                { "Films", ShowFilms },
                { "Menu", ShowMenuConfirmation }
            }, addBackChoice: true);

            var visitorsMenuChoice = visitorsMenu.MakeChoice();

            if (visitorsMenuChoice.Key == "Terug")
                Program.StartChoice();
            else
                visitorsMenuChoice.Value();
        }

        private static void ShowFilms()
        {
            ConsoleHelper.LogoType = LogoType.Films;
            ConsoleHelper.Breadcrumb = $"Genres: {(_genres?.Count > 0 ? string.Join(", ", _genres.Select(x => x.Key)) : "Alle")}\n" +
                $"   Tijden: {(_dayAndTimes.Key != null && _dayAndTimes.Value.Length > 0 ? $"{_dayAndTimes.Key} om {string.Join(", ", _dayAndTimes.Value)}" : "Alle") }";

            var movies = new Dictionary<string, Action>();

            movies["Filters"] = ShowFilters;

            foreach (var movie in JsonHelper.Movies)
            {
                if (_genres?.Count > 0 && movie.Genres.Intersect(_genres.Select(x => x.Key)).Count() == 0)
                    continue;
                if (_dayAndTimes.Key != null && _dayAndTimes.Value.Length > 0 && !movie.Days[_dayAndTimes.Key.ToLower()].Intersect(_dayAndTimes.Value).Any())
                    continue;
                movies[movie.Title] = null;
            }


            var movieMenu = new ChoiceMenu(movies, true);

            var movieChoice = movieMenu.MakeChoice();

            movieChoice.Value?.Invoke();

            if (movieChoice.Key == "Terug")
                ShowVisitorMenu();
            else if (movieChoice.Value == null)
                ShowFilmInfo(movieChoice.Key);
        }

        private static void ShowFilmInfo(string movieName)
        {
            ConsoleHelper.LogoType = LogoType.Films;
            ConsoleHelper.Breadcrumb = $"Films / {movieName}";
            
            var movie = JsonHelper.Movies.FirstOrDefault(x => x.Title == movieName);

            var text = $"Titel: {movie.Title}\n\nOmschrijving: { movie.Description}\n\nZaal: {movie.Room}\n\n_______________________________________\n";       

            var filmChoiceMenu = new ChoiceMenu(new Dictionary<string, Action>
            {
                { "Koop tickets", null },
            }, addBackChoice: true, text);

            var filmChoice = filmChoiceMenu.MakeChoice();

            if (filmChoice.Key == "Terug")
                ShowFilms();
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

            var dayChoice = dayChoiceMenu.MakeChoice();

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

        private static void ShowMenuConfirmation()
        {
            ConsoleHelper.LogoType = LogoType.Films;
            ConsoleHelper.Breadcrumb = null;

            var menuConfirmationChoice = ChoiceMenu.CreateConfirmationChoiceMenu("   Wil je het menu assortiment bekijken?\n").MakeChoice();

            if (menuConfirmationChoice.Key == "Ja")
                ShowMenu();
            else
                ShowVisitorMenu(); // TODO: Go to payment screen.
        }

        private static void ShowMenu()
        {
            ConsoleHelper.LogoType = LogoType.Menu;
            ConsoleHelper.Breadcrumb = null;

            var categoryChoices = new Dictionary<string, Action>();

            foreach (var category in JsonHelper.Menu)
                categoryChoices[category.Name] = null;

            categoryChoices["Winkelmand legen"] = null;
            categoryChoices["Ga door"] = null;

            var categoryChoiceMenu = new ChoiceMenu(categoryChoices, true, MenuCartText);

            var categoryChoice = categoryChoiceMenu.MakeChoice();

            if (categoryChoice.Key == "Terug")
                ShowVisitorMenu();
            else if (categoryChoice.Key == "Winkelmand legen")
            {
                _menuCart.Clear();
                ShowMenu();
            }
            else if (categoryChoice.Key == "Ga door")
                ShowMenu();
            else
                ShowCategoryItems(JsonHelper.Menu.FirstOrDefault(x => x.Name == categoryChoice.Key));            
        }

        private static void ShowCategoryItems(MenuCategory category)
        {
            ConsoleHelper.LogoType = LogoType.Menu;
            ConsoleHelper.Breadcrumb = $"Categorie: {category.Name}";

            var itemChoices = new Dictionary<string, Action>();

            foreach (var item in category.MenuItems)
                itemChoices[item.Name] = null;

            var itemChoiceMenu = new ChoiceMenu(itemChoices, true, MenuCartText);

            var itemChoice = itemChoiceMenu.MakeChoice();

            if (itemChoice.Key == "Terug")
                ShowMenu();
            else
                ShowItemTypes(category, itemChoice.Key);
        }

        private static void ShowItemTypes(MenuCategory category, string menuItem)
        {
            ConsoleHelper.LogoType = LogoType.Menu;
            ConsoleHelper.Breadcrumb += $"\n   Product: {menuItem}";

            var itemTypes = category.MenuItems.FirstOrDefault(x => x.Name == menuItem).ItemTypes;

            var typeChoices = new Dictionary<string, Action>();

            foreach (var type in itemTypes)
                typeChoices[$"{type.Key} - {type.Value:0.00} euro"] = null;

            var typeChoiceMenu = new ChoiceMenu(typeChoices, true, MenuCartText);

            var typeChoice = typeChoiceMenu.MakeChoice();

            if (typeChoice.Key == "Terug")
                ShowCategoryItems(category);
            else
            {
                var itemTypeData = itemTypes.FirstOrDefault(x => typeChoice.Key.Contains(x.Key));
                _menuCart.Add(new KeyValuePair<string, decimal>($"{menuItem} {itemTypeData.Key.ToLower()} - {itemTypeData.Value:0.00}", itemTypeData.Value));
                ShowMenu();
            }
        }
    }
}
