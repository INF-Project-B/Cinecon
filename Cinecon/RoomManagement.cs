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
    }
}
