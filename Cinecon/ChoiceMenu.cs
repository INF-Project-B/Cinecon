using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinecon
{
    public class ChoiceMenu
    {
        private readonly List<KeyValuePair<string, Action>> _choices;

        public ChoiceMenu(Dictionary<string, Action> choices, bool addBackChoice = false)
        {
            if (addBackChoice)
                choices["Terug"] = null;

            _choices = choices.ToList();
        }

        public KeyValuePair<string, Action> MakeChoice()
        {
            Console.CursorVisible = false;

            int index = 0;

            WriteMenu(index);

            ConsoleKeyInfo keyInfo;
            var lastPressedTime = DateTime.Now;

            while (true)
            {
                keyInfo = Console.ReadKey(true);

                if (DateTime.Now > lastPressedTime.AddMilliseconds(50))
                {
                    if (keyInfo.Key == ConsoleKey.DownArrow)
                    {
                        index = index + 1 < _choices.Count ? index + 1 : 0;
                        WriteMenu(index);
                    }

                    if (keyInfo.Key == ConsoleKey.UpArrow)
                    {
                        index = index - 1 >= 0 ? index - 1 : _choices.Count - 1;
                        WriteMenu(index);
                    }

                    if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                        return _choices[index];
                    }

                    if (keyInfo.Key == ConsoleKey.Backspace && _choices.Any(x => x.Key == "Terug"))
                        return _choices.FirstOrDefault(x => x.Key == "Terug");
                }

                lastPressedTime = DateTime.Now;
            }
        }

        private void WriteMenu(int index)
        {
            Console.Clear();
            ConsoleHelper.WriteLogo(ConsoleColor.Red);
            ConsoleHelper.WriteBreadcrumb();

            if (ConsoleHelper.LogoType == LogoType.Cinecon)
                ConsoleHelper.ColorWriteLine("  Welkom bij Cinecon!\n  Bent u een medewerker of bezoeker?\n", ConsoleColor.Yellow);

            foreach (var choice in _choices)
            {
                if (choice.Key == "Terug" || choice.Key == "Exit")
                    Console.WriteLine();
                if (choice.Key == _choices.ElementAt(index).Key)
                    ConsoleHelper.ColorWriteLine($" > {choice.Key}", ConsoleColor.Red);
                else
                    Console.WriteLine($"  {choice.Key}");
            }
        }
    }
}
