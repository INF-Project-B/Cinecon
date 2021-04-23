﻿using System;
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

        private static void ShowReservations()
        {
            var reservationCodes = new Dictionary<string, Action>();
            
            foreach (var reservations in JsonHelper.Reservations)
                reservationCodes[reservations.Code] = null;

            var reservationsCodeChoice = new ChoiceMenu(reservationCodes, addBackChoice: true).MakeChoice();

            if (reservationsCodeChoice.Key == "Terug")
                ShowEmployeesMenu();
            else
                ShowCodeInfo(JsonHelper.Reservations.FirstOrDefault(x => x.Code == reservationsCodeChoice.Key));
        }

        private static void ShowCodeInfo(Reservation reservation)
        {
            var reservationDescription = $"   Code {reservation.Code} is {(reservation.IsActivated ? "actief" : "inactief")}\n" +
                $"   Er is betaald met {reservation.PaymentMethod}.\n   {(reservation.Seats.Count > 1 ? "Stoelen: " : "Stoel: ")}" +
                string.Join(", ", reservation.Seats.Select(x => $"{x.Row}{x.Number}")) + ".\n";

            var backChoice = new ChoiceMenu(new Dictionary<string, Action>
            {
                { "Activeer code", () => reservation.IsActivated = true}
            }, addBackChoice: true, reservationDescription, ConsoleColor.Yellow).MakeChoice();

            if (backChoice.Key == "Terug")
                ShowReservations();
            else
            {
                backChoice.Value();
                ShowCodeInfo(reservation);
            }
        }
    }
}