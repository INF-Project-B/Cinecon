using System;
using System.Collections.Generic;
using System.Text;

namespace Cinecon
{
    public class VisitorsMenu
    {
        public static void ShowVisitorMenu()
        {
            ConsoleHelper.ColorWriteLine("Bezoekers Menu", ConsoleColor.Yellow);
            var visitorsMenu = new ChoiceMenu(new Dictionary<string, Action>
            {
                { "Films", Films },
                { "Snacks", Snacks },
            });
            var visitors = visitorsMenu.MakeChoice();
            visitors.Value();
        }

        public static void Films()
        {

        }

        public static void Snacks()
        {

        }
    }
}
