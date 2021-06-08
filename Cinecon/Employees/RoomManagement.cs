using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinecon
{
    public static class RoomManagement
    {
        public static Room GetRoom(int roomNumber, DateTime date)
            => JsonHelper.Days.FirstOrDefault(x => x.Item1 == date).Item2.FirstOrDefault(x => x.Number == roomNumber);

        public static List<Room> GetAllRoomsByNumber(int roomNumber)
            => JsonHelper.Days.Select(x => x.Item2).Select(x => x.FirstOrDefault(x => x.Number == roomNumber)).ToList();

        private static class RoomCreation
        {
            public static Room CreateRoomSetup()
            {
                ConsoleHelper.WriteLogoAndBreadcrumb();

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
                    if (int.TryParse(ConsoleHelper.ReadLineWithText("   Wat is het zaalnummer? -> ", writeLine: false), out result))
                    {
                        if (JsonHelper.Days.FirstOrDefault().Item2.FirstOrDefault(x => x.Number == result) == null)
                            break;
                        else
                        {
                            ConsoleHelper.WriteLogoAndBreadcrumb();
                            ConsoleHelper.ColorWriteLine("   Er bestaat al een zaal met dit nummer.\n", ConsoleColor.Red);
                        }
                    }
                    else
                    {
                        ConsoleHelper.WriteLogoAndBreadcrumb();
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
                        ConsoleHelper.WriteLogoAndBreadcrumb();
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
                        ConsoleHelper.WriteLogoAndBreadcrumb();
                        ConsoleHelper.ColorWriteLine("   Vul a.u.b. een positief getal in.\n", ConsoleColor.Red);
                    }
                }
                Console.CursorVisible = false;
                return result;
            }
        }

        private static DateTime ShowDateOptions()
        {
            var dateOptions = new Dictionary<string, Action>();

            foreach (var date in JsonHelper.Days)
            {
                var day = date.Item1.ToString("dddd dd MMMM");
                dateOptions[char.ToUpper(day[0]) + day[1..]] = null;
            }

            var dateOptionsMenu = new ChoiceMenu(dateOptions, true, "   Voor welke dag wilt u de zalen bekijken?\n");

            var dateChoice = dateOptionsMenu.MakeChoice();

            if (dateChoice.Key == "Terug")
            {
                EmployeesMenu.ShowEmployeesMenu();
                return default;
            }
            else if (dateChoice.Value == null)
                return JsonHelper.Days.FirstOrDefault(x => x.Item1.ToString("dddd dd MMMM") == dateChoice.Key.ToLower()).Item1;
            else
                return default;
        }

        public static void ShowRoomOptions(DateTime date = default)
        {
            ConsoleHelper.Breadcrumb = "Zalen";

            date = date == default ? ShowDateOptions() : date;

            ConsoleHelper.Breadcrumb = $"Zalen / {date.DayOfWeek} - {date:dd/MM}";

            var roomOptions = new Dictionary<string, Action>
            {
                ["Zaal toevoegen"] = ShowAddRoom
            };

            foreach (var room in JsonHelper.Days.FirstOrDefault(x => x.Item1.Date == date.Date).Item2.OrderBy(x => x.Number))
                roomOptions[$"Zaal {room.Number}"] = null;

            var roomOptionsMenu = new ChoiceMenu(roomOptions, addBackChoice: true, roomOptions.Count > 0 ? "" : "   Geen zalen gevonden.");

            var roomOptionsChoice = roomOptionsMenu.MakeChoice();

            if (roomOptionsChoice.Key == "Terug")                            
                ShowRoomOptions();
            else if (roomOptionsChoice.Value != null)
                roomOptionsChoice.Value();
            else
                ShowRoomManagement(int.Parse(roomOptionsChoice.Key.Replace("Zaal ", "")), date);
        }

        private static void ShowRoomManagement(int roomNumber, DateTime date)
        {
            Console.Clear();

            ConsoleHelper.Breadcrumb = $"Zalen / {date.DayOfWeek} - {date:dd/MM} / Zaal {roomNumber}";

            var rooms = GetAllRoomsByNumber(roomNumber);
            var room = GetRoom(roomNumber, date);

            var roomInfoOptions = new ChoiceMenu(new Dictionary<string, Action>
            {
                { "Zaalnummer wijzigen", null },
                { "Aantal rijen wijzigen", null },
                { "Aantal stoelen per rij wijzigen", null },
                { "Zaal verwijderen", null }
            }, addBackChoice: true, $"   Zaal: {room.Number}\n   Beschikbare stoelen: {room.Seats.Count(x => !x.IsTaken)}/{room.Seats.Count}\n   Aantal rijen: {room.TotalRows}\n   Stoelen per rij: {room.SeatsPerRow}\n   Aantal stoelen: {room.TotalSeats}\n");

            var roomInfoOptionsChoice = roomInfoOptions.MakeChoice();
            
            ConsoleHelper.WriteLogo(ConsoleColor.Red);                       

            switch (roomInfoOptionsChoice.Key)
            {
                case "Terug":
                    Console.Clear();
                    ShowRoomOptions(date);
                    return;
                case "Zaalnummer wijzigen":                    
                    ConsoleHelper.Breadcrumb += " / Zaalnummer wijzigen";
                    ConsoleHelper.WriteBreadcrumb();
                    int newNumber = RoomCreation.AskRoomNumber();
                    foreach (var r in rooms)
                        r.Number = newNumber;
                    JsonHelper.UpdateJsonFiles();
                    ShowRoomManagement(newNumber, date);
                    return;
                case "Aantal rijen wijzigen":
                    ConsoleHelper.Breadcrumb += " / Aantal rijen wijzigen";
                    ConsoleHelper.WriteBreadcrumb();
                    int newRows = RoomCreation.AskTotalRows();
                    foreach (var r in rooms)
                        r.TotalRows = newRows;
                    JsonHelper.UpdateJsonFiles();
                    break;
                case "Aantal stoelen per rij wijzigen":
                    ConsoleHelper.Breadcrumb += " / Aantal stoelen per rij wijzigen";
                    ConsoleHelper.WriteBreadcrumb();
                    int newSeatsPerRow = RoomCreation.AskSeatsPerRow();
                    foreach (var r in rooms)
                        r.SeatsPerRow = newSeatsPerRow;
                    JsonHelper.UpdateJsonFiles();
                    break;
                case "Zaal verwijderen":
                    ConsoleHelper.Breadcrumb += " / Zaal verwijderen";
                    ConsoleHelper.WriteBreadcrumb();
                    Console.Clear();
                    var confirmationChoice = ChoiceMenu.CreateConfirmationChoiceMenu("   Weet je zeker dat je deze zaal wilt verwijderen?\n").MakeChoice();
                    if (confirmationChoice.Key == "Ja")
                    {
                        foreach (var day in JsonHelper.Days)
                            day.Item2.Remove(day.Item2.FirstOrDefault(x => x.Number == room.Number));
                        JsonHelper.UpdateJsonFiles();
                        Console.Clear();
                        ShowRoomOptions();
                    }
                    break;
            }

            ShowRoomManagement(roomNumber, date);
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
                foreach (var day in JsonHelper.Days)
                    day.Item2.Add(room);
                JsonHelper.UpdateJsonFiles();
            }

            ShowRoomOptions();
        }
    }
}
