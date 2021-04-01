using System;

namespace Cinecon
{
    public class ConsoleHelper
    {
        public static void ColorWriteLine(string text, ConsoleColor color)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = currentColor;
        }

        public static void WriteLogo(ConsoleColor color = ConsoleColor.White)
        {
            ColorWriteLine(@"   ____ _                            
  / ___(_)_ __   ___  ___ ___  _ __  
 | |   | | '_ \ / _ \/ __/ _ \| '_ \ 
 | |___| | | | |  __/ (_| (_) | | | |
  \____|_|_| |_|\___|\___\___/|_| |_|
                                     ", color);
        }
    }
}
