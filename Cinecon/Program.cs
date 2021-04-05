using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinecon
{
    public class Program
    {
        static void Main()
        {
            // Load JSON files into project.
            JsonHelper.LoadJson();

            ConsoleHelper.WriteLogo(LogoType.Cinecon, ConsoleColor.Yellow);

            Console.WriteLine("Welkom bij Cinecon!\nBent u een medewerker of bezoeker?");

            // We maken hier een nieuwe ChoiceMenu object aan en stoppen het in een variable.
            // Hierbij moeten we een Dictionary<string, Action> als argument geven. Dit zijn de keuzes die de gebruiker kan selecteren
            // en wat er gebeurt als ze deze keuzes selecteren.
            var choiceMenu = new ChoiceMenu(new Dictionary<string, Action>
            {
                { "Bezoeker", VisitorsMenu.ShowVisitorMenu },
                { "Medewerker", () => ConsoleHelper.ColorWriteLine("Yo ik ben een medewerker", ConsoleColor.Yellow) },
            });

            // Hiermee kunnen we een keuzescherm genereren waarin de gebruiker moet kiezen tussen de keuzes die we in de dictionary hierboven hebt aangegeven.
            var choice = choiceMenu.MakeChoice();

            // Value is de functie van de geselecteerde keuze die we in de dictionary hebben gestopt.
            // We kunnen de functie callen door Value() te doen.
            // Voorbeeld: Als de gebruiker kiest voor "bezoeker", dan wordt de functie "VisitorsMenu.ShowVisitorMenu" gecalled.
            choice.Value();
        }
    }    
}