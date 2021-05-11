using System;
using System.Collections.Generic;

namespace Cinecon
{
    public static class Tickets
    {
        public static void ShowTicketMenu(string movieName)
        {
            ConsoleHelper.WriteLogo(ConsoleColor.Red);
            ConsoleHelper.WriteBreadcrumb();
         
            ConsoleHelper.Breadcrumb = $"Films / Koop Tickets";


           

            Console.Write($"Selecteer datum en tijd: ");
                
         
            Console.Write($"Aantal: ");
            var quantity = Console.ReadLine();
            if (!int.TryParse(quantity, out int num))
                Console.WriteLine("Kies opnieuw het AANTAL kaartjes!");
            else
                Console.WriteLine($"U heeft gekozen voor de film; {movieName}\n\nDaarbij heeft u gekozen voor ({quantity}) kaartjes,\ndie geldig zijn op TEST om Test uur!\n\nGaat u hiermee akkoord?");
            
           var ticketChoiceMenu = new ChoiceMenu(new Dictionary<string, Action>
            {
                {"Akkoord", null } 
                
            }, addBackChoice: true);
            Console.Clear();

            var dealChoice = ticketChoiceMenu.MakeChoice();
            
            if (dealChoice.Key == "Akkoord")
                Seats.ChooseSeats();
                
            else if (dealChoice.Key == "Terug")
                ShowTicketMenu(movieName);
                    
        }
    }
}
    