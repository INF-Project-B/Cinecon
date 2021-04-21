using System;
using System.Collections.Generic;

namespace Cinecon
{
    public class Program
    {
        private static void Main()
        {
            Console.Clear();

            JsonHelper.LoadJson();

            StartChoice();
        }

        public static void StartChoice()
        {
            ConsoleHelper.LogoType = LogoType.Cinecon;

            var choiceMenu = new ChoiceMenu(new Dictionary<string, Action>
            {
                { "Bezoeker", VisitorsMenu.ShowVisitorMenu },
                { "Medewerker", () => { ConsoleHelper.LogoType = LogoType.Employee; ConsoleHelper.WriteLogo(ConsoleColor.Red); } },
                { "Exit", () => Environment.Exit(0) },
            });

            var choice = choiceMenu.MakeChoice();

            choice.Value();
        }
    }    
}