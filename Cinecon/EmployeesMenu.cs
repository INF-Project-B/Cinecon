using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinecon
{
    public class EmployeesMenu
    {
        public static void ShowEmployeesMenu()
        {
            ConsoleHelper.LogoType = LogoType.Employee;

            var employeesMenu = new ChoiceMenu(new Dictionary<string, Action>
            {
                { "Reservatiecodes", ShowReservations }
            }, addBackChoice: true);

            var employeesMenuChoice = employeesMenu.MakeChoice();

            if (employeesMenuChoice.Key == "Terug")
                Program.StartChoice();
            else
                employeesMenuChoice.Value();
        }

        public static void ShowReservations()
        {

            var reservationsCode = new Dictionary<string, Action>();

            var realReservations = new Dictionary<string, Reservation>();
            
            foreach (var reservations in JsonHelper.Reservations)
            {
                reservationsCode[reservations.Code] = null;
                realReservations[reservations.Code] = reservations;
            }

            var reservationsCodeChoice = new ChoiceMenu(reservationsCode, addBackChoice: true).MakeChoice().Key;

            if (reservationsCodeChoice == "Terug")
                ShowEmployeesMenu();
            else;
                ShowCodeInfo(realReservations[reservationsCodeChoice]);
        }

        public static void ShowCodeInfo(Reservation checkReservation)
        {
            var isActivated = checkReservation.IsActivated ? $"Code {checkReservation.Code} is actief" : $"Code {checkReservation.Code} is inactief";

            var length = checkReservation.Seats.Count > 1 ? "Stoelen " : "stoel ";

            var reservationDescription = $"  {isActivated}\n  Er is betaald met {checkReservation.PaymentMethod}\n  {length}";

            for (int i = 0;i < checkReservation.Seats.Count;i++)
            {
                reservationDescription += $"{checkReservation.Seats[i].Row}{checkReservation.Seats[i].Number}";

                reservationDescription += i < checkReservation.Seats.Count - 1 ? ", " : "\n";
            }
            var backChoice = new ChoiceMenu(new Dictionary<string, Action>
            {
                { "Activeer code", () => checkReservation.IsActivated = true}
            }, addBackChoice: true, reservationDescription, ConsoleColor.Yellow).MakeChoice();


            if (backChoice.Key == "Terug")
                ShowReservations();
            else
            {
                backChoice.Value();
                ShowReservations();
            }
        }
    }
}