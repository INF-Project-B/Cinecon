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
        public static void StartPaymentProcess(DateTime date)
        {
            var movie = ReservationSystem.SelectedMovie;

            ConsoleHelper.LogoType = LogoType.Films;
            ConsoleHelper.Breadcrumb = $"Films / {movie.Title} / Koop tickets / Betaling";
            ConsoleHelper.WriteLogoAndBreadcrumb();
            Console.CursorVisible = true;

            string name;
            string email;
            while (true)
            {
                name = ConsoleHelper.ReadLineWithText("   Onder welke naam wilt u reserveren? -> ", writeLine: false);
                var correctName = ChoiceMenu.CreateConfirmationChoiceMenu($"   Wilt u verder gaan met de naam {name}?\n");
                if (string.IsNullOrEmpty(name))
                {
                    ConsoleHelper.WriteLogoAndBreadcrumb();
                    ConsoleHelper.ColorWriteLine("   Vul a.u.b. een naam in\n", ConsoleColor.Red);
                }
                else
                {
                    Console.Clear();
                    var correctNameChoice = correctName.MakeChoice();
                    if (correctNameChoice.Key == "Ja")
                    {
                        Console.Clear();
                        break;
                    }
                    else
                        ConsoleHelper.WriteLogoAndBreadcrumb();
                }
            }

            ConsoleHelper.WriteLogoAndBreadcrumb();
            var emailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            while (true)
            {
                email = ConsoleHelper.ReadLineWithText("   Wat is uw e-mail adres? -> ", writeLine: false);
                var correctMail = ChoiceMenu.CreateConfirmationChoiceMenu($"   Wilt u verder gaan met de volgende mail: {email}?\n");

                if (!emailRegex.Match(email).Success)
                {
                    ConsoleHelper.WriteLogoAndBreadcrumb();
                    ConsoleHelper.ColorWriteLine("   Voer a.u.b. een geldige e-mail adres in.\n", ConsoleColor.Red);
                }
                else 
                {
                    Console.Clear();
                    var correctMailChoice = correctMail.MakeChoice();
                    if (correctMailChoice.Key == "Ja")
                    {
                        Console.Clear();
                        break;
                    }
                    else
                        ConsoleHelper.WriteLogoAndBreadcrumb();
                }
            }

            Console.CursorVisible = false;

            var paymentMethods = new Dictionary<string, Action>();

            foreach (var method in JsonHelper.ReservationData.PaymentMethods)
                paymentMethods[method] = null;

            var paymentMethodChoice = new ChoiceMenu(paymentMethods, true, "   Kies een betaalmethode\n").MakeChoice();

            if (paymentMethodChoice.Key == "Terug")
                ReservationSystem.ShowSeats(date);
            else
                FinishPaymentProcess(movie, paymentMethodChoice.Key, name, email, date);
        }

        private static void FinishPaymentProcess(Movie movie, string paymentMethod, string name, string email, DateTime date)
        {
            ConsoleHelper.LogoType = LogoType.Films;
            ConsoleHelper.Breadcrumb = $"Films / {movie.Title} / Koop tickets / Betaling";

            var confirmationChoice = ChoiceMenu.CreateConfirmationChoiceMenu(
                $"   Is de ingevulde data correct?\n\n   Film: {movie.Title}\n   Naam: {name}\n   E-mail: {email}\n   Betaalmethode: {paymentMethod}\n{MenuSystem.MenuCartText}\n").MakeChoice();

            if (confirmationChoice.Key != "Ja")
            {
                StartPaymentProcess(date);
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
                Date = date,
                Movie = movie,
                Seats = ReservationSystem.SelectedSeats
            };

            JsonHelper.ReservationData.Reservations.Add(reservation);

            foreach (var seat in RoomManagement.GetRoom(movie.Room, date).Seats)
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
            foreach (var seat in reservation.Seats.OrderBy(x => $"{x.Row}{x.Number}"))
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
                .Replace("[DATE]", reservation.Date.ToString("dddd dd MMMM"))
                .Replace("[TIME]", ReservationSystem.Time)
                .Replace("[MENU]", MenuSystem.MenuCartText)
                .Replace("[MOVIE_DESCRIPTION]", reservation.Movie.Description)
                .Replace("[ROOM]", reservation.Movie.Room.ToString());

            smtpClient.Send(emailData.Email, reservation.Email, emailData.Subject, body);
        }
    }
}
