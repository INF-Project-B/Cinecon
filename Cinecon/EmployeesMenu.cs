using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinecon
{
    public static class EmployeesMenu
    {
        public static void ShowEmployeesMenu()
        {
            ConsoleHelper.LogoType = LogoType.Employee;
            ConsoleHelper.Breadcrumb = null;

            var employeesMenu = new ChoiceMenu(new Dictionary<string, Action>
            {
                { "Reserveringscodes", ShowReservations },
                { "Zalen", ShowRoomOptions }
            }, addBackChoice: true);

            var employeesMenuChoice = employeesMenu.MakeChoice();

            if (employeesMenuChoice.Key == "Terug")
                Program.StartChoice();
            else
                employeesMenuChoice.Value();
        }

        private static void ShowRoomOptions()
        {
            ConsoleHelper.Breadcrumb = "Zalen"; 

            var roomManagementOptions = new ChoiceMenu(new Dictionary<string, Action>
            {
                { "Zaal toevoegen", ShowAddRoom },
                { "Zaal management", ShowRooms }
            }, addBackChoice: true);

            var roomManagementOptionsChoice = roomManagementOptions.MakeChoice();

            if (roomManagementOptionsChoice.Key == "Terug")
                ShowEmployeesMenu();
            else
                roomManagementOptionsChoice.Value();
        }

        private static void ShowRooms()
        {
            ConsoleHelper.Breadcrumb = "Zalen / Zaal management";

            var rooms = new Dictionary<string, Action>();
            for (int i = 0; i < JsonHelper.Rooms.Count; i++)
                rooms[$"Zaal {i + 1}"] = null;

            var roomOptions = new ChoiceMenu(rooms, addBackChoice: true);

            var roomOptionsChoice = roomOptions.MakeChoice();

            if (roomOptionsChoice.Key == "Terug")
                ShowRoomOptions();
            else                
                ShowRooms(); // Temporary.

            // TODO: Show room info when selected.
        }

        private static void ShowAddRoom()
        {
            var room = RoomManagement.CreateRoomSetup();

            Console.Clear();

            var roomOptions = new ChoiceMenu(new Dictionary<string, Action>
            {
                { "Ja", null },
                { "Nee", null }
            }, text: $"   Weet je zeker dat je de nieuwe zaal wilt toevoegen?\n\n   Aantal rijen: {room.TotalRows}\n   Stoelen per rij: {room.SeatsPerRow}\n");

            var roomOptionsChoice = roomOptions.MakeChoice();

            if (roomOptionsChoice.Key == "Ja")
            {
                JsonHelper.Rooms.Add(room);
                JsonHelper.UpdateJsonFiles();
            }

            ShowRoomOptions();
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