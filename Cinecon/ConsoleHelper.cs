using System;

namespace Cinecon
{
    public class ConsoleHelper
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
