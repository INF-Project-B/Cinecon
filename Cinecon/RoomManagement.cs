using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinecon
{
    public static class RoomManagement
    {
        public static Room GetRoom(int roomNumber)
            => JsonHelper.Rooms.FirstOrDefault(x => x.Number == roomNumber);

        public static Room CreateRoomSetup()
        {
            int roomNumber = JsonHelper.Rooms.Count + 1;
            int totalRows;
            int seatsPerRow;

            void WriteRoomMenu()
            {
                Console.Clear();
                Console.CursorVisible = true;
                ConsoleHelper.WriteLogo(ConsoleColor.Red);
                ConsoleHelper.Breadcrumb = "Zalen / Zaal toevoegen";
                ConsoleHelper.WriteBreadcrumb();
                ConsoleHelper.ColorWrite("   Nieuwe zaal: ", ConsoleColor.Red);
                ConsoleHelper.ColorWrite($"{roomNumber}\n\n", ConsoleColor.White);
            }

            WriteRoomMenu();

            while (true)
            {
                if (int.TryParse(ConsoleHelper.ReadLineWithText("   Hoeveel rijen bevat de zaal? (Min. 1, max. 26) -> ", writeLine: false), out totalRows))
                {
                    totalRows = Math.Clamp(totalRows, 1, 26);
                    break;
                }
                else
                {
                    WriteRoomMenu();
                    ConsoleHelper.ColorWriteLine("   Vul a.u.b. een positief getal in.\n", ConsoleColor.Red);
                }
            }

            while (true)
            {
                if (int.TryParse(ConsoleHelper.ReadLineWithText("   Hoeveel stoelen bevat elke rij? (Min. 1) -> ", writeLine: false), out seatsPerRow))
                {
                    seatsPerRow = Math.Clamp(seatsPerRow, 1, int.MaxValue);
                    break;
                }
                else
                {
                    WriteRoomMenu();
                    ConsoleHelper.ColorWriteLine("   Vul a.u.b. een positief getal in.\n", ConsoleColor.Red);
                }
            }

            var seats = new List<Seat>();

            for (int i = 0; i < totalRows; i++)
                for (int j = 0; j < seatsPerRow; j++)
                    seats.Add(new Seat { Row = ((char)(65 + i)).ToString(), Number = j + 1, IsTaken = false });

            return new Room
            {
                Movies = new List<Movie>(),
                Number = roomNumber,
                Seats = seats,
                TotalSeats = seats.Count,
                TotalRows = totalRows,
                SeatsPerRow = seatsPerRow
            };
        }

        public static void ShowRoomOptions()
        {
            ConsoleHelper.Breadcrumb = "Zalen";

            var roomManagementOptions = new ChoiceMenu(new Dictionary<string, Action>
            {
                { "Zaal toevoegen", ShowAddRoom },
                { "Zaal management", ShowRooms }
            }, addBackChoice: true);

            var roomManagementOptionsChoice = roomManagementOptions.MakeChoice();

            if (roomManagementOptionsChoice.Key == "Terug")
                EmployeesMenu.ShowEmployeesMenu();
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
                ShowRoomManagement(int.Parse(roomOptionsChoice.Key.Replace("Zaal ", "")));
        }

        private static void ShowRoomManagement(int roomNumber)
        {
            ConsoleHelper.Breadcrumb = $"Zalen / Zaal management / Zaal {roomNumber}";

            var room = GetRoom(roomNumber);

            var roomInfoOptions = new ChoiceMenu(new Dictionary<string, Action>
            {
                { "Aantal rijen wijzigen", null },
                { "Aantal stoelen per rij wijzigen", null },
                { "Zaal verwijderen", null }
            }, addBackChoice: true, $"   Zaal: {room.Number}\n   Aantal rijen: {room.TotalRows}\n   Stoelen per rij: {room.SeatsPerRow}\n   Aantal stoelen: {room.TotalSeats}\n");

            var roomInfoOptionsChoice = roomInfoOptions.MakeChoice();

            if (roomInfoOptionsChoice.Key == "Terug")
                ShowRooms();
        }

        private static void ShowAddRoom()
        {
            var room = CreateRoomSetup();

            Console.Clear();

            var roomOptions = ChoiceMenu.CreateConfirmationChoiceMenu($"   Weet je zeker dat je de nieuwe zaal wilt toevoegen?\n\n" +
                $"   Aantal rijen: {room.TotalRows}\n   Stoelen per rij: {room.SeatsPerRow}\n");

            var roomOptionsChoice = roomOptions.MakeChoice();

            if (roomOptionsChoice.Key == "Ja")
            {
                JsonHelper.Rooms.Add(room);
                JsonHelper.UpdateJsonFiles();
            }

            ShowRoomOptions();
        }
    }
}
