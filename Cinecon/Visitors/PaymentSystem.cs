using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cinecon
{
    public static class PaymentSystem
    {
        public static void StartPaymentProcess()
        {
            var movie = ReservationSystem.SelectedMovie;

            ConsoleHelper.LogoType = LogoType.Films;
            ConsoleHelper.Breadcrumb = $"Films / {movie.Title} / Koop tickets / Betaling";

            string name;
            string email;

            do
            {
                ConsoleHelper.WriteLogoAndBreadcrumb();
                name = ConsoleHelper.ReadLineWithText("   Onder welke naam wil je reserveren? -> ", writeLine: false);
            }
            while (string.IsNullOrEmpty(name));

            var invalidEmail = false;
            var emailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            do
            {
                ConsoleHelper.WriteLogoAndBreadcrumb();
                if (invalidEmail)
                    ConsoleHelper.ColorWriteLine("   Voer a.u.b. een valide e-mail adres in.\n", ConsoleColor.Red);
                email = ConsoleHelper.ReadLineWithText("   Wat is jouw e-mail adres? -> ", writeLine: false);
                invalidEmail = true;
            }
            while (!emailRegex.Match(email).Success);

            Console.Clear();

            var paymentMethods = new Dictionary<string, Action>();

            foreach (var method in JsonHelper.ReservationData.PaymentMethods)
                paymentMethods[method] = null;

            var paymentMethodChoice = new ChoiceMenu(paymentMethods, true, "   Kies een betaalmethode\n").MakeChoice();

            if (paymentMethodChoice.Key == "Terug")
                ReservationSystem.ShowSeats();
            else
                FinishPaymentProcess(movie, paymentMethodChoice.Key, name, email);
        }

        private static void FinishPaymentProcess(Movie movie, string paymentMethod, string name, string email)
        {
            ConsoleHelper.LogoType = LogoType.Films;
            ConsoleHelper.Breadcrumb = $"Films / {movie.Title} / Koop tickets / Betaling";

            var confirmationChoice = ChoiceMenu.CreateConfirmationChoiceMenu(
                $"   Is de ingevulde data correct?\n\n   Film: {movie.Title}\n   Naam: {name}\n   E-mail: {email}\n   Betaalmethode: {paymentMethod}\n{MenuSystem.MenuCartText}\n").MakeChoice();

            if (confirmationChoice.Key != "Ja")
            {
                StartPaymentProcess();
                return;
            }

            var reservation = new Reservation
            {
                Name = name,
                Email = email,
                Code = GenerateRandomCode(),
                PaymentMethod = paymentMethod,
                IsActivated = false,
                Room = movie.Room,
                Movie = movie,
                Seats = ReservationSystem.SelectedSeats
            };

            JsonHelper.ReservationData.Reservations.Add(reservation);

            foreach (var seat in RoomManagement.GetRoom(movie.Room).Seats)
                if (reservation.Seats.Any(x => x.Row == seat.Row && x.Number == seat.Number))
                    seat.IsTaken = true;
                
            JsonHelper.UpdateJsonFiles();

            _ = Task.Run(() => SendReservationEmail(reservation));

            new ChoiceMenu(new Dictionary<string, Action>
            {
                { "Afronden", Program.StartChoice }
            }, text: $"   Jouw betaling met {paymentMethod} is succesvol afgerond. Jouw reserveringscode is {reservation.Code}.\n   Je zult straks ook een e-mail krijgen met jouw reserveringscode.\n   " +
                $"Bedankt en tot de voorstelling!\n").MakeChoice().Value();
        }

        private static string GenerateRandomCode()
        {
            string randomCode;
            var r = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            do
            {
                randomCode = new string(Enumerable.Range(1, 5).Select(_ => chars[r.Next(chars.Length)]).ToArray());
            }
            while (JsonHelper.ReservationData.Reservations.Any(x => x.Code == randomCode));

            return randomCode;
        }

        private static void SendReservationEmail(Reservation reservation)
        {
            var emailData = JsonHelper.EmailData;

            using var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(emailData.Email, emailData.Password),
                EnableSsl = true
            };
            var seats = $"{(reservation.Seats.Count() > 1 ? "stoelen zijn: " : "stoel is ")}";
            var count = 1;
            foreach (var seat in reservation.Seats)
            {
                seats += $"{seat.Row}{seat.Number}{(reservation.Seats.Count()-1 == count ? " en " : reservation.Seats.Count() > count ? ", " : "")}";
                count++;
            }

            var body = emailData.Body
                .Replace("[NAME]", reservation.Name)
                .Replace("[CODE]", reservation.Code)
                .Replace("[EMAIL]", reservation.Email)
                .Replace("[PAYMENT_METHOD]", reservation.PaymentMethod)
                .Replace("[MOVIE_TITLE]", reservation.Movie.Title)
                .Replace("[SEATS]", seats)
                .Replace("[MOVIE_DESCRIPTION]", reservation.Movie.Description)
                .Replace("[ROOM]", reservation.Movie.Room.ToString());

            smtpClient.Send(emailData.Email, reservation.Email, emailData.Subject, body);
        }
    }
}
