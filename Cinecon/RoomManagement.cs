using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinecon
{
    public static class RoomManagement
    {
        public static Room GetRoom(int roomNumber)
            => JsonHelper.Rooms.FirstOrDefault(x => x.Number == roomNumber);

        public static class RoomCreation
        {
            public static Room CreateRoomSetup()
            {
                WriteRoomMenu();

                int roomNumber = AskRoomNumber();
                int totalRows = AskTotalRows();
                int seatsPerRow = AskSeatsPerRow();

                return new Room
                {
                    Movies = new List<Movie>(),
                    Number = roomNumber,
                    TotalRows = totalRows,
                    SeatsPerRow = seatsPerRow
                };
            }

            public static int AskRoomNumber()
            {
                Console.CursorVisible = true;
                int result;
                while (true)
                {
                    if (int.TryParse(ConsoleHelper.ReadLineWithText("   Wat is de zaalnummer? -> ", writeLine: false), out result))
                    {
                        if (GetRoom(result) == null)
                            break;
                        else
                        {
                            WriteRoomMenu();
                            ConsoleHelper.ColorWriteLine("   Er bestaat al een zaal met dit nummer.\n", ConsoleColor.Red);
                        }
                            
                    }
                    else
                    {
                        WriteRoomMenu();
                        ConsoleHelper.ColorWriteLine("   Vul a.u.b. een getal in.\n", ConsoleColor.Red);
                    }
                }
                Console.CursorVisible = false;
                return result;
            }

            public static int AskTotalRows()
            {
                Console.CursorVisible = true;
                int result;
                while (true)
                {
                    if (int.TryParse(ConsoleHelper.ReadLineWithText("   Hoeveel rijen bevat de zaal? (Min. 1, max. 26) -> ", writeLine: false), out result))
                    {
                        result = Math.Clamp(result, 1, 26);
                        break;
                    }
                    else
                    {
                        WriteRoomMenu();
                        ConsoleHelper.ColorWriteLine("   Vul a.u.b. een positief getal in.\n", ConsoleColor.Red);
                    }
                }
                Console.CursorVisible = false;
                return result;
            }

            public static int AskSeatsPerRow()
            {
                Console.CursorVisible = true;
                int result;
                while (true)
                {
                    if (int.TryParse(ConsoleHelper.ReadLineWithText("   Hoeveel stoelen bevat elke rij? (Min. 1) -> ", writeLine: false), out result))
                    {
                        result = Math.Clamp(result, 1, int.MaxValue);
                        break;
                    }
                    else
                    {
                        WriteRoomMenu();
                        ConsoleHelper.ColorWriteLine("   Vul a.u.b. een positief getal in.\n", ConsoleColor.Red);
                    }
                }
                Console.CursorVisible = false;
                return result;
            }

            private static void WriteRoomMenu()
            {
                Console.Clear();
                ConsoleHelper.WriteLogo(ConsoleColor.Red);
                ConsoleHelper.WriteBreadcrumb();
            }
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
            foreach (var room in JsonHelper.Rooms.OrderBy(x => x.Number))
                rooms[$"Zaal {room.Number}"] = null;

            var roomOptions = new ChoiceMenu(rooms, addBackChoice: true, rooms.Count > 0 ? "" : "   Geen zalen gevonden.");

            var roomOptionsChoice = roomOptions.MakeChoice();

            if (roomOptionsChoice.Key == "Terug")
                ShowRoomOptions();
            else
                ShowRoomManagement(int.Parse(roomOptionsChoice.Key.Replace("Zaal ", "")));
        }

        private static void ShowRoomManagement(int roomNumber)
        {
            Console.Clear();

            ConsoleHelper.Breadcrumb = $"Zalen / Zaal management / Zaal {roomNumber}";

            var room = GetRoom(roomNumber);

            var roomInfoOptions = new ChoiceMenu(new Dictionary<string, Action>
            {
                { "Zaalnummer wijzigen", null },
                { "Aantal rijen wijzigen", null },
                { "Aantal stoelen per rij wijzigen", null },
                { "Zaal verwijderen", null }
            }, addBackChoice: true, $"   Zaal: {room.Number}\n   Aantal rijen: {room.TotalRows}\n   Stoelen per rij: {room.SeatsPerRow}\n   Aantal stoelen: {room.TotalSeats}\n");

            var roomInfoOptionsChoice = roomInfoOptions.MakeChoice();
            
            ConsoleHelper.WriteLogo(ConsoleColor.Red);                       

            switch (roomInfoOptionsChoice.Key)
            {
                case "Terug":
                    Console.Clear();
                    ShowRooms();
                    break;
                case "Zaalnummer wijzigen":                    
                    ConsoleHelper.Breadcrumb += " / Zaalnummer wijzigen";
                    ConsoleHelper.WriteBreadcrumb();
                    room.Number = RoomCreation.AskRoomNumber();
                    JsonHelper.UpdateJsonFiles();
                    break;
                case "Aantal rijen wijzigen":
                    ConsoleHelper.Breadcrumb += " / Aantal rijen wijzigen";
                    ConsoleHelper.WriteBreadcrumb();
                    room.TotalRows = RoomCreation.AskTotalRows();
                    JsonHelper.UpdateJsonFiles();
                    break;
                case "Aantal stoelen per rij wijzigen":
                    ConsoleHelper.Breadcrumb += " / Aantal stoelen per rij wijzigen";
                    ConsoleHelper.WriteBreadcrumb();
                    room.SeatsPerRow = RoomCreation.AskSeatsPerRow();
                    JsonHelper.UpdateJsonFiles();
                    break;
                case "Zaal verwijderen":
                    ConsoleHelper.Breadcrumb += " / Zaal verwijderen";
                    ConsoleHelper.WriteBreadcrumb();
                    Console.Clear();
                    var confirmationChoice = ChoiceMenu.CreateConfirmationChoiceMenu("   Weet je zeker dat je deze zaal wilt verwijderen?\n").MakeChoice();
                    if (confirmationChoice.Key == "Ja")
                    {
                        JsonHelper.Rooms.Remove(room);
                        JsonHelper.UpdateJsonFiles();
                        Console.Clear();
                        ShowRooms();
                    }
                    else
                        ShowRoomManagement(roomNumber);
                    break;
            }

            ShowRoomManagement(room.Number);
        }

        private static void ShowAddRoom()
        {
            ConsoleHelper.Breadcrumb = "Zalen / Zaal toevoegen";

            var room = RoomCreation.CreateRoomSetup();

            Console.Clear();

            var roomOptionsChoice = ChoiceMenu.CreateConfirmationChoiceMenu($"   Weet je zeker dat je de nieuwe zaal wilt toevoegen?\n\n" +
                $"   Zaal: {room.Number}\n   Aantal rijen: {room.TotalRows}\n   Stoelen per rij: {room.SeatsPerRow}\n   Aantal stoelen: {room.TotalSeats}\n").MakeChoice();

            if (roomOptionsChoice.Key == "Ja")
            {
                JsonHelper.Rooms.Add(room);
                JsonHelper.UpdateJsonFiles();
            }

            ShowRoomOptions();
        }
    }
}
