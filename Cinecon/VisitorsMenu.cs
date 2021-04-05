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

        }
    }
}
