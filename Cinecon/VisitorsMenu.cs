using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinecon
{
    public class VisitorsMenu
    {
        public static void ShowVisitorMenu()
        {
            ConsoleHelper.WriteLogo(LogoType.Visitor, ConsoleColor.Yellow);

            var visitorsMenu = new ChoiceMenu(new Dictionary<string, Action>
            {
                { "Films", ShowFilms },
                { "Menu", ShowMenu },
            });

            var visitorsMenuChoice = visitorsMenu.MakeChoice();

            visitorsMenuChoice.Value();
        }

        private static void ShowFilms()
        {
            ConsoleHelper.WriteLogo(LogoType.Films, ConsoleColor.Yellow);
        }

        private static void ShowMenu()
        {
            ConsoleHelper.WriteLogo(LogoType.Menu, ConsoleColor.Yellow);

            var categoryChoices = new Dictionary<string, Action>();

            foreach (var category in JsonHelper.Menu)
                categoryChoices[category.Name] = null;

            var categoryChoiceMenu = new ChoiceMenu(categoryChoices);

            var categoryChoice = categoryChoiceMenu.MakeChoice();

            ShowCategoryItems(categoryChoice.Key);            
        }

        private static void ShowCategoryItems(string categoryName)
        {
            ConsoleHelper.WriteLogo(LogoType.Menu, ConsoleColor.Yellow);

            ConsoleHelper.ColorWriteLine(categoryName, ConsoleColor.Yellow);

            var itemChoices = new Dictionary<string, Action>();

            var category = JsonHelper.Menu.FirstOrDefault(x => x.Name == categoryName);

            foreach (var item in category.MenuItems)
                itemChoices[item.Name] = null;

            var itemChoiceMenu = new ChoiceMenu(itemChoices);

            var itemChoice = itemChoiceMenu.MakeChoice();

            ShowItemTypes(category, itemChoice.Key);
        }

        private static void ShowItemTypes(MenuCategory category, string item)
        {
            ConsoleHelper.WriteLogo(LogoType.Menu, ConsoleColor.Yellow);

            ConsoleHelper.ColorWriteLine($"{category.Name}\n\n{item}", ConsoleColor.Yellow);

            var typeChoices = new Dictionary<string, Action>();

            foreach (var type in category.MenuItems.FirstOrDefault(x => x.Name == item).ItemTypes)
                typeChoices[$"{type.Key} - {type.Value:0.00} euro"] = null;

            var typeChoiceMenu = new ChoiceMenu(typeChoices);

            var typeChoice = typeChoiceMenu.MakeChoice();
        }
    }
}
