﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinecon
{
    public class ChoiceMenu
    {
        private readonly List<KeyValuePair<string, Action>> _choices;

        public ChoiceMenu(Dictionary<string, Action> choices, bool addBackChoice = false, string text = null)
        {
            if (addBackChoice)
                choices["Terug"] = null;

            _choices = choices.ToList();
        }

        private void ChoiceSetup(List<KeyValuePair<string, Action>> preselected = null)
        {
            Console.CursorVisible = false;
            WriteMenu(0, preselected);
        }

        public KeyValuePair<string, Action> MakeChoice()
        {
            ChoiceSetup();

            int index = 0;

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

        public List<KeyValuePair<string, Action>> MakeMultipleChoice(List<KeyValuePair<string, Action>> preselectedGenres = null)
        {
            ChoiceSetup(preselectedGenres);

            int index = 0;

            var choices = preselectedGenres ?? new List<KeyValuePair<string, Action>>();

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
                        WriteMenu(index, choices);
                    }

                    if (keyInfo.Key == ConsoleKey.UpArrow)
                    {
                        index = index - 1 >= 0 ? index - 1 : _choices.Count - 1;
                        WriteMenu(index, choices);
                    }

                    if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        Console.Clear();

                        var choice = _choices[index];

                        if (new[] { "Terug", "Ga door" }.Any(x => x == choice.Key))
                            return choices;

                        if (!choices.Contains(choice))
                            choices.Add(choice);
                        else
                            choices.Remove(choice);

                        WriteMenu(index, choices);
                    }

                    if (keyInfo.Key == ConsoleKey.Backspace && _choices.Any(x => x.Key == "Terug" || x.Key == "Ga door"))
                        return choices;
                }

                lastPressedTime = DateTime.Now;
            }
        }

        private void WriteMenu(int index, List<KeyValuePair<string, Action>> selectedChoices = null)
        {
            Console.Clear();
            ConsoleHelper.WriteLogo(ConsoleColor.Red);
            ConsoleHelper.WriteBreadcrumb();

            if (ConsoleHelper.LogoType == LogoType.Cinecon)
                ConsoleHelper.ColorWriteLine("  Welkom bij Cinecon!\n  Bent u een medewerker of bezoeker?\n", ConsoleColor.Yellow);

            foreach (var choice in _choices)
            {
                if (new[] { "Terug", "Exit", "Filters" }.Any(x => x == choice.Key))
                    Console.WriteLine();

                bool choiceIsSelected = selectedChoices != null && selectedChoices.Contains(choice);

                if (choice.Key == _choices.ElementAt(index).Key)
                {
                    if (choiceIsSelected)
                    {
                        ConsoleHelper.ColorWrite($" > ", ConsoleColor.Red);
                        ConsoleHelper.ColorWrite(choice.Key + "\n", ConsoleColor.Green);
                    }
                    else
                        ConsoleHelper.ColorWriteLine($" > {choice.Key}", ConsoleColor.Red);
                }
                else
                {
                    if (choice.Key == "Filters")
                        ConsoleHelper.ColorWriteLine($"  {choice.Key}", ConsoleColor.Yellow);
                    else
                    {
                        if (choiceIsSelected)
                            ConsoleHelper.ColorWriteLine($"  {choice.Key}", ConsoleColor.Green);
                        else
                            Console.WriteLine($"  {choice.Key}");
                    }
                }
            }
        }
    }
}
