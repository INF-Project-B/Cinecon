using System;

namespace Cinecon
{
    public static class ConsoleHelper
    {
        public static LogoType LogoType { get; set; } = LogoType.Cinecon;
        public static string Breadcrumb { get; set; }

        public static void ColorWriteLine(string text, ConsoleColor color)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = currentColor;
        }

        public static void ColorWrite(string text, ConsoleColor color)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = currentColor;
        }

        public static void WriteBreadcrumb()
        {
            if (!string.IsNullOrEmpty(Breadcrumb))
                ColorWriteLine("   " + Breadcrumb + "\n", ConsoleColor.Yellow);
        }

        public static string ReadLineWithText(string text, ConsoleColor color = ConsoleColor.White,  bool writeLine = true)
        {
            if (writeLine)
                ColorWriteLine(text, color);
            else
                ColorWrite(text, color);
            return Console.ReadLine();
        }

        public static void WriteLogoAndBreadcrumb()
        {
            Console.Clear();
            WriteLogo(ConsoleColor.Red);
            WriteBreadcrumb();
        }

        public static string TranslateDate(string date)
        {
            return date switch 
            {
                "Monday" => "Maandag",
                "Tuesday" => "Dinsdag",
                "Wednesday" => "Woensdag",
                "Thursday" => "Donderdag",
                "Friday" => "Vrijdag",
                "Saturday" => "Zaterdag",
                "Sunday" => "Zondag",
                "maandag" => "Maandag",
                "dinsdag" => "Dinsdag",
                "woensdag" => "Woensdag",
                "donderdag" => "Donderdag",
                "vrijdag" => "Vrijdag",
                "zaterdag" => "Zaterdag",
                "zondag" => "Zondag",
                "January" => "januari",
                "February" => "februari",
                "March" => "maart",
                "April" => "april",
                "May" => "mei",
                "June" => "juni",
                "July" => "juli",
                "August" => "augustus",
                "September" => "september",
                "October" => "oktober",
                "November" => "november",
                "December" => "december",
                _ => date
            };
        }

        public static void WriteLogo(ConsoleColor color = ConsoleColor.White)
        {
            string text = LogoType switch
            {
                LogoType.Cinecon => @"   ____ _                            
  / ___(_)_ __   ___  ___ ___  _ __  
 | |   | | '_ \ / _ \/ __/ _ \| '_ \ 
 | |___| | | | |  __/ (_| (_) | | | |
  \____|_|_| |_|\___|\___\___/|_| |_|
                                     ",
                LogoType.Visitor => @"  ____                     _             
 | __ )  ___ _______   ___| | _____ _ __ 
 |  _ \ / _ \_  / _ \ / _ \ |/ / _ \ '__|
 | |_) |  __// / (_) |  __/   <  __/ |   
 |____/ \___/___\___/ \___|_|\_\___|_|   
                                         ",
                LogoType.Employee => @"  __  __          _                        _             
 |  \/  | ___  __| | _____      _____ _ __| | _____ _ __ 
 | |\/| |/ _ \/ _` |/ _ \ \ /\ / / _ \ '__| |/ / _ \ '__|
 | |  | |  __/ (_| |  __/\ V  V /  __/ |  |   <  __/ |   
 |_|  |_|\___|\__,_|\___| \_/\_/ \___|_|  |_|\_\___|_|   
                                                         ",
                LogoType.Menu => @"  __  __                  
 |  \/  | ___ _ __  _   _ 
 | |\/| |/ _ \ '_ \| | | |
 | |  | |  __/ | | | |_| |
 |_|  |_|\___|_| |_|\__,_|
                          ",
                LogoType.Films => @"  _____ _ _               
 |  ___(_) |_ __ ___  ___ 
 | |_  | | | '_ ` _ \/ __|
 |  _| | | | | | | | \__ \
 |_|   |_|_|_| |_| |_|___/
                          ",
                _ => ""
            };

            ColorWriteLine(text + "\n_______________________________________\n", color);
        }        
    }

    public enum LogoType
    {
        Cinecon,
        Visitor,
        Employee,
        Menu,
        Films
    }
}
