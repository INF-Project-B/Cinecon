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
                { "Films", ShowFilms },
                { "Menu", ShowMenu }
            }, true);

            var visitorsMenuChoice = visitorsMenu.MakeChoice();

            if (visitorsMenuChoice.Key == "Terug")
                Program.StartChoice();
            else
                visitorsMenuChoice.Value();
        }

        private static void ShowFilms()
        {
            ConsoleHelper.LogoType = LogoType.Films;
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

            ConsoleHelper.Breadcrumb = $"  Categorie: {category.Name}\n";

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

            ConsoleHelper.Breadcrumb += $"  Product: {item}\n";

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
