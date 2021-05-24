using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinecon
{
    public class ChoiceMenu
    {
        private readonly List<KeyValuePair<string, Action>> _choices;
        private readonly string _text;
        private readonly ConsoleColor _textColor;
        private string[] _highlighted;

        public ChoiceMenu(Dictionary<string, Action> choices, bool addBackChoice = false, string text = null, ConsoleColor textColor = ConsoleColor.White)
        {
            if (addBackChoice)
                choices["Terug"] = null;

            _choices = choices.ToList();
            _text = text;
            _textColor = textColor;
        }

        public static ChoiceMenu CreateConfirmationChoiceMenu(string text = null, ConsoleColor color = ConsoleColor.White)
        {
            return new ChoiceMenu(new Dictionary<string, Action>
            {
                { "Ja", null },
                { "Nee", null }
            }, text: text, textColor: color);
        }

        private void ChoiceSetup(List<KeyValuePair<string, Action>> preselected = null)
        {
            Console.CursorVisible = false;
            WriteMenu(0, preselected);
        }

        public KeyValuePair<string, Action> MakeChoice(string[] highlighted = null)
        {
            _highlighted = highlighted;

            ChoiceSetup();

            int index = 0;

            while (true)
            {
                Console.CursorTop = 0;

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.DownArrow:
                        index = index + 1 < _choices.Count ? index + 1 : 0;
                        WriteMenu(index);
                        break;
                    case ConsoleKey.UpArrow:
                        index = index - 1 >= 0 ? index - 1 : _choices.Count - 1;
                        WriteMenu(index);
                        break;
                    case ConsoleKey.Enter:
                        Console.Clear();
                        return _choices[index];
                    case ConsoleKey.Backspace:
                        if (_choices.Any(x => x.Key == "Terug"))
                        {
                            Console.Clear();
                            return _choices.FirstOrDefault(x => x.Key == "Terug");
                        }
                        break;
                }
            }
        }

        public List<KeyValuePair<string, Action>> MakeMultipleChoice(List<KeyValuePair<string, Action>> preselected = null)
        {
            ChoiceSetup(preselected);

            int index = 0;

            var choices = preselected ?? new List<KeyValuePair<string, Action>>();

            while (true)
            {
                Console.CursorTop = 0;

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.DownArrow:
                        index = index + 1 < _choices.Count ? index + 1 : 0;
                        WriteMenu(index, choices);
                        break;
                    case ConsoleKey.UpArrow:
                        index = index - 1 >= 0 ? index - 1 : _choices.Count - 1;
                        WriteMenu(index, choices);
                        break;
                    case ConsoleKey.Enter:
                        var choice = _choices[index];
                        if (new[] { "Terug", "Ga door" }.Any(x => x == choice.Key))
                        {
                            Console.Clear();
                            return choices;
                        }
                        if (!choices.Contains(choice))
                            choices.Add(choice);
                        else
                            choices.Remove(choice);
                        WriteMenu(index, choices);
                        break;
                    case ConsoleKey.Backspace:
                        if (_choices.Any(x => x.Key == "Terug" || x.Key == "Ga door"))
                        {
                            Console.Clear();
                            return choices;
                        }
                        break;
                }
            }
        }

        private void WriteMenu(int index, List<KeyValuePair<string, Action>> selectedChoices = null)
        {
            ConsoleHelper.WriteLogo(ConsoleColor.Red);
            ConsoleHelper.WriteBreadcrumb();

            if (!string.IsNullOrEmpty(_text))
                ConsoleHelper.ColorWriteLine(_text, _textColor);
            
            foreach (var choice in _choices)
            {
                if (new[] { "Terug", "Exit", "Ga door", "Winkelmand legen" }.Any(x => x == choice.Key))
                    Console.WriteLine();

                bool choiceIsSelected = selectedChoices != null && selectedChoices.Contains(choice);
                bool isHighighted = _highlighted?.Contains(choice.Key) ?? false;

                if (choice.Key == _choices.ElementAt(index).Key)
                {
                    if (choiceIsSelected || isHighighted)
                    {
                        ConsoleHelper.ColorWrite($" > ", ConsoleColor.Red);
                        ConsoleHelper.ColorWrite(choice.Key + "\n", ConsoleColor.Green);
                    }
                    else
                        ConsoleHelper.ColorWriteLine($" > {choice.Key}", ConsoleColor.Red);
                }
                else
                {
                    if (choiceIsSelected || isHighighted)
                        ConsoleHelper.ColorWriteLine($"   {choice.Key}", ConsoleColor.Green);
                    else
                        Console.WriteLine($"   {choice.Key}");
                }

                if (choice.Key == "Filters" || choice.Key == "Zaal toevoegen")
                    Console.WriteLine();
            }
        }
    }
}
