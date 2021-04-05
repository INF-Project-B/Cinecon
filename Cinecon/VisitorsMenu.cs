using System;
using System.Collections.Generic;

namespace Cinecon
{
    public class VisitorsMenu
    {
        public static void ShowVisitorMenu()
        {
            ConsoleHelper.ColorWriteLine("Bezoekers Menu", ConsoleColor.Yellow);

            var visitorsMenu = new ChoiceMenu(new Dictionary<string, Action>
            {
                { "Films", ShowFilms },
                { "Menu", ShowMenu },
            });

            var visitors = visitorsMenu.MakeChoice();

            visitors.Value();
        }

        public static void ShowFilms()
        {

        }

        public static void ShowMenu()
        {

        }
    }
}
