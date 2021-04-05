using System;
using System.Collections.Generic;

namespace Cinecon
{
    public class Program
    {
        static void Main()
        {
            // Load JSON files into project.
            JsonHelper.LoadJson();

            StartChoice();
        }

        public static void StartChoice()
        {
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