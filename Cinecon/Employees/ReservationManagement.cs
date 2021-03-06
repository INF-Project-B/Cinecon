using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinecon
{
    public static class ReservationManagement
    {
        public static void ShowReservations()
        {
            var reservationCodes = new Dictionary<string, Action>
            {
                ["Zoek op code"] = null,
                ["Terug"] = null
            };

            var activatedCodes = new string[JsonHelper.ReservationData.Reservations.Count(x => x.IsActivated)];

            var count = 0;
            foreach (var reservation in JsonHelper.ReservationData.Reservations)
            {
                reservationCodes[reservation.Code] = null;
                if (reservation.IsActivated)
                    activatedCodes[count++] = reservation.Code;
            }

            var reservationsCodeChoice = new ChoiceMenu(reservationCodes, false, "   Geactiveerde codes worden met groen aangegeven.\n", ConsoleColor.Yellow).MakeChoice(activatedCodes);

            if (reservationsCodeChoice.Key == "Zoek op code")
                ShowSearchCode(reservationCodes);
            else if (reservationsCodeChoice.Key == "Terug")
                EmployeesMenu.ShowEmployeesMenu();
            else
                ShowCodeInfo(JsonHelper.ReservationData.Reservations.FirstOrDefault(x => x.Code == reservationsCodeChoice.Key));
        }

        private static void ShowSearchCode(Dictionary<string, Action> reservationCodes)
        {
            Console.CursorVisible = true;
            ConsoleHelper.WriteLogoAndBreadcrumb();

            var searchAgain = ChoiceMenu.CreateConfirmationChoiceMenu("   De ingevulde code was niet gevonden. Wilt u nog eens zoeken?\n");

            string code;
            while (true)
            {
                code = ConsoleHelper.ReadLineWithText("   Voer a.u.b. een code in met een lengte van 5 letters en/of getallen. -> ", writeLine: false);

                if (reservationCodes.ContainsKey(code))
                {
                    Console.Clear();
                    ShowCodeInfo(JsonHelper.ReservationData.Reservations.FirstOrDefault(x => x.Code == code));
                    break;
                }
                else if (code.Length != 5)
                {
                    ConsoleHelper.WriteLogoAndBreadcrumb();
                    ConsoleHelper.ColorWriteLine("   Vul a.u.b. een code in met een lengte van 5\n", ConsoleColor.Red);
                }
                else
                {
                    Console.Clear();
                    var searchAgainChoice = searchAgain.MakeChoice();
                    if (searchAgainChoice.Key == "Nee")
                        ShowReservations();
                    else
                        ShowSearchCode(reservationCodes);
                }
            }
            Console.CursorVisible = false;
        }

        private static void ShowCodeInfo(Reservation reservation)
        {
            var reservationDescription = $"   Code {reservation.Code} is {(reservation.IsActivated ? "actief" : "inactief")}\n" +
                $"   Betalingsmethode: {reservation.PaymentMethod}\n   {(reservation.Seats.Count > 1 ? "Stoelen: " : "Stoel: ")}" +
                string.Join(", ", reservation.Seats.Select(x => $"{x.Row}{x.Number}")) + "\n" +
                $"   Naam: {reservation.Name}\n   E-mail: {reservation.Email}\n";

            var backChoice = new ChoiceMenu(new Dictionary<string, Action>
            {
                { reservation.IsActivated ? "Deactiveer code" : "Activeer code", () => reservation.IsActivated = !reservation.IsActivated }
            }, addBackChoice: true, reservationDescription, ConsoleColor.Yellow).MakeChoice();

            if (backChoice.Key == "Terug")
                ShowReservations();
            else
            {
                backChoice.Value();
                JsonHelper.UpdateJsonFiles();
                ShowCodeInfo(reservation);
            }
        }
    }
}
