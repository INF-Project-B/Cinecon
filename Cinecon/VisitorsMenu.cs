using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinecon
{
    public class VisitorsMenu
    {
        public static void ShowVisitorMenu()
        {
            ConsoleHelper.LogoType = LogoType.Visitor;

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

        private static void ShowFilms(List<KeyValuePair<string, Action>> genres = null)
        {
            ConsoleHelper.LogoType = LogoType.Films;
            ConsoleHelper.Breadcrumb = "Filters: ";

            ConsoleHelper.Breadcrumb += genres?.Count > 0 ? string.Join(", ", genres.Select(x => x.Key)) : "Geen";

            var movies = new Dictionary<string, Action>();

            foreach (var movie in JsonHelper.Movies)
            {
                if (genres?.Count > 0 && movie.Genres.Intersect(genres.Select(x => x.Key)).Count() == 0)
                    continue;
                movies[movie.Title] = null;
            }

            movies["Filters"] = null;

            var movieMenu = new ChoiceMenu(movies, true);

            var movieChoice = movieMenu.MakeChoice();

            if (movieChoice.Key == "Terug")
                ShowVisitorMenu();
            else if (movieChoice.Key == "Filters")
                ShowGenres(genres);
            else
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

        private static void ShowGenres(List<KeyValuePair<string, Action>> genres)
        {
            ConsoleHelper.LogoType = LogoType.Films;
            ConsoleHelper.Breadcrumb = "Films / Filters";

            var genreChoices = new Dictionary<string, Action>();

            foreach (var genre in JsonHelper.Genres)
                genreChoices[genre] = null;

            var genreChoiceMenu = new ChoiceMenu(genreChoices, true);

            var selectedGenres = genreChoiceMenu.MakeMultipleChoice(genres);

            ShowFilms(selectedGenres);
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
