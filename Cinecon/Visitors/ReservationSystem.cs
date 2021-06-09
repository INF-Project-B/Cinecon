using System;
using System.Collections.Generic;
using System.Linq;
using static Cinecon.MovieSystem;

namespace Cinecon
{
    public static class ReservationSystem
    {
        public static Movie SelectedMovie { get; private set; }
        public static List<Seat> SelectedSeats { get; private set; }
        public static string Time { get; private set; }
        private static int _ticketsAmount;

        public static void ChooseTicketsAmount(Movie movie, DateTime date, string time, bool error = false)
        {
            SelectedMovie = movie;

            Console.CursorVisible = true;

            Time = time[0..5];

            Console.Clear();
            ConsoleHelper.Breadcrumb = $"Films / {movie.Title} / {date:dddd} / {time} / Tickets";
            ConsoleHelper.WriteLogoAndBreadcrumb();

            if (error)
                ConsoleHelper.ColorWrite("   Vul graag een positief getal in binnen het aantal beschikbare stoelen.\n\n", ConsoleColor.Red);

            Room room = RoomManagement.GetRoom(movie.Room, date);

            int availableSeatsCount = room.Seats.Count(x => !x.IsTaken);

            Console.Write($"   Hoeveel tickets wil je bestellen? ({availableSeatsCount} beschikbaar) -> ");

            if (!int.TryParse(Console.ReadLine(), out _ticketsAmount) || _ticketsAmount < 1 || _ticketsAmount > availableSeatsCount)
            {
                ChooseTicketsAmount(movie, date, time, true);
                return;
            }

            Console.CursorVisible = false;

            ShowTicketMenu(date, time);
        }

        private static void ShowTicketMenu(DateTime date, string time)
        {
            ConsoleHelper.LogoType = LogoType.Films;
            ConsoleHelper.Breadcrumb = $"Films / {SelectedMovie.Title} / Dagen en tijd / Aantal";

            var confirmationMenu = ChoiceMenu.CreateConfirmationChoiceMenu($"   Film: {SelectedMovie.Title}\n   Aantal tickets: {_ticketsAmount}\n   " +
            $"Dag en tijd: {ConsoleHelper.TranslateDate(date.ToString("dddd"))} om {time} uur.\n\n   Gaat je hiermee akkoord?\n");

            Console.Clear();

            var confirmationChoice = confirmationMenu.MakeChoice();

            if (confirmationChoice.Key == "Ja")
                ShowSeats(date);
            else
                ShowFilmInfo(SelectedMovie.Title);
        }

        public static void ShowSeats(DateTime date)
        {
            var room = RoomManagement.GetRoom(SelectedMovie.Room, date);

            var seats = new List<Dictionary<string, Action>>();

            var seatNumber = 0;
            for (int i = 0; i < room.TotalRows; i++)
            {
                var row = new Dictionary<string, Action>();
                for (int j = 0; j < room.SeatsPerRow; j++)
                {
                    row[$"{room.Seats[seatNumber].Row}{(room.Seats[seatNumber].Number < 10 ? "0" : "")}{room.Seats[seatNumber].Number}"] = null;
                    seatNumber++;
                }
                seats.Add(row);
            }

            seats.Add(new Dictionary<string, Action> { { "Ga door", ShowFilms } });
            seats.Add(new Dictionary<string, Action> { { "Terug", null } });

            var seatChoiceMenu = new ChoiceMenu(seats, $"   Zaal: {room.Number}\n   Kies a.u.b. {_ticketsAmount} aantal stoelen door op enter te drukken\n", ConsoleColor.Yellow);

            var seatChoices = seatChoiceMenu.MakeAllChoice(_ticketsAmount, room: room);

            SelectedSeats = seatChoices.Item2;

            if (seatChoices.Item1 == "Terug")
                ShowFilmInfo(SelectedMovie.Title);
            if (seatChoices.Item1 == "Ga door")
                MenuSystem.ShowMenuConfirmation(date);
        }
    }
}
