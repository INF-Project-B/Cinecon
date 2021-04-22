using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinecon
{
    public class VisitorsMenu
    {
        private static List<KeyValuePair<string, Action>> _genres;

        public static void ShowVisitorMenu()
        {
            ConsoleHelper.LogoType = LogoType.Visitor;
            ConsoleHelper.Breadcrumb = null;

            var visitorsMenu = new ChoiceMenu(new Dictionary<string, Action>
            {
                { "Films", null },
                { "Menu", ShowMenu }
            }, addBackChoice: true);

            var visitorsMenuChoice = visitorsMenu.MakeChoice();

            if (visitorsMenuChoice.Key == "Terug")
                Program.StartChoice();
            else if (visitorsMenuChoice.Key == "Films")
                ShowFilms();
            else
                visitorsMenuChoice.Value();
        }

        private static void ShowFilms()
        {
            ConsoleHelper.LogoType = LogoType.Films;
            ConsoleHelper.Breadcrumb = $"Filters: {(_genres?.Count > 0 ? string.Join(", ", _genres.Select(x => x.Key)) : "Geen")}";

            var movies = new Dictionary<string, Action>();

            foreach (var movie in JsonHelper.Movies)
            {
                if (_genres?.Count > 0 && movie.Genres.Intersect(_genres.Select(x => x.Key)).Count() == 0)
                    continue;
                movies[movie.Title] = null;
            }

            movies["Filters"] = ShowFilters;

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
            // TODO: Add functionality for when the user makes a selection.
        }

        private static void ShowFilters()
        {
            ConsoleHelper.LogoType = LogoType.Films;
            ConsoleHelper.Breadcrumb = "Films / Filters";

            var filtersChoiceMenu = new ChoiceMenu(new Dictionary<string, Action>
            {
                { "Genres", ShowGenres },
                { "Datum en tijd", null },
            }, addBackChoice: true);

            var filtersChoice = filtersChoiceMenu.MakeChoice();

            filtersChoice.Value?.Invoke();

            if (filtersChoice.Key == "Terug")
                ShowFilms();
        }

        private static void ShowGenres()
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

        private static void ShowMenu()
        {
            ConsoleHelper.LogoType = LogoType.Menu;
            ConsoleHelper.Breadcrumb = null;

            var categoryChoices = new Dictionary<string, Action>();

            foreach (var category in JsonHelper.Menu)
                categoryChoices[category.Name] = null;

            var categoryChoiceMenu = new ChoiceMenu(categoryChoices, true);

            var categoryChoice = categoryChoiceMenu.MakeChoice();

            if (categoryChoice.Key == "Terug")
                ShowVisitorMenu();
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

            var itemChoiceMenu = new ChoiceMenu(itemChoices, true);

            var itemChoice = itemChoiceMenu.MakeChoice();

            if (itemChoice.Key == "Terug")
                ShowMenu();
            else
                ShowItemTypes(category, itemChoice.Key);
        }

        private static void ShowItemTypes(MenuCategory category, string item)
        {
            ConsoleHelper.LogoType = LogoType.Menu;
            ConsoleHelper.Breadcrumb += $"Product: {item}";

            var typeChoices = new Dictionary<string, Action>();

            foreach (var type in category.MenuItems.FirstOrDefault(x => x.Name == item).ItemTypes)
                typeChoices[$"{type.Key} - {type.Value:0.00} euro"] = null;

            var typeChoiceMenu = new ChoiceMenu(typeChoices, true);

            var typeChoice = typeChoiceMenu.MakeChoice();

            if (typeChoice.Key == "Terug")
                ShowCategoryItems(category);
            else 
            { 
                // Coming soon 
            }                
        }
    }
}
