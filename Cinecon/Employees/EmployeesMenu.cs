﻿using System;
using System.Collections.Generic;

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
                { "Reserveringscodes", ReservationManagement.ShowReservations },
                { "Zalen", RoomManagement.ShowRoomOptions }
            }, addBackChoice: true);

            var employeesMenuChoice = employeesMenu.MakeChoice();

            if (employeesMenuChoice.Key == "Terug")
                Program.StartChoice();
            else
                employeesMenuChoice.Value();
        }
    }
}