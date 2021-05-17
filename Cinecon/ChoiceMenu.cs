using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinecon
{
    public class ChoiceMenu
    {
        private readonly List<List<KeyValuePair<string, Action>>> _2dChoices;
        private readonly List<KeyValuePair<string, Action>> _choices;
        private readonly string _text;
        private readonly ConsoleColor _textColor;

        public ChoiceMenu(Dictionary<string, Action> choices, bool addBackChoice = false, string text = null, ConsoleColor textColor = ConsoleColor.White)
        {
            if (addBackChoice)
                choices["Terug"] = null;

            _choices = choices.ToList();
            _text = text;
            _textColor = textColor;
        }

        public ChoiceMenu(List<Dictionary<string, Action>> twodChoices, bool addBackChoice, string text = null, ConsoleColor textColor = ConsoleColor.White)
        {

            _2dChoices = twodChoices.Select(x => x.ToList()).ToList();
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

        public KeyValuePair<string, Action> MakeChoice()
        {
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

        public List<KeyValuePair<string, Action>> MakeMultipleChoice(List<KeyValuePair<string, Action>> preselectedGenres = null)
        {
            ChoiceSetup(preselectedGenres);

            int index = 0;

            var choices = preselectedGenres ?? new List<KeyValuePair<string, Action>>();

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

        public Tuple<string, List<Seat>> MakeAllChoice(List<List<KeyValuePair<string, Action>>> preselectedGenres = null, Room room = null)
        {
            Console.CursorVisible = false;
            WriteAllMenu(0, 0, room: room);

            int indexY = 0;

            int indexX = 0;

            var allChoices = preselectedGenres ?? new List<List<KeyValuePair<string, Action>>>();

            var choices = new List<KeyValuePair<string, Action>>();

            var count = 0;

            foreach (var row in allChoices)
                foreach (var collumn in row)
                    choices[count++] = allChoices[allChoices.IndexOf(row)][row.IndexOf(collumn)];

            while (true)
            {
                Console.CursorTop = 0;

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.LeftArrow:
                        if (indexX - 1 >= 0)
                            indexX--;
                        else
                        {
                            indexX = _2dChoices[indexY].Count - 1;
                            indexY = indexY - 1 >= 0 ? indexY - 1 : 0;
                        }
                        WriteAllMenu(indexY, indexX, choices, room: room);
                        break;
                    case ConsoleKey.RightArrow:
                        if (indexX + 1 < _2dChoices[indexY].Count)
                            indexX++;
                        else
                        {
                            indexX = 0;
                            indexY = indexY + 1 < _2dChoices.Count ? indexY + 1 : _2dChoices.Count - 1;
                        }
                        WriteAllMenu(indexY, indexX, choices, room: room);
                        break;
                    case ConsoleKey.DownArrow:
                        
                        if (indexY + 1 < _2dChoices.Count)
                        {
                            indexX = indexY + 1 == _2dChoices.Count-2 ? 0 : indexX;
                            indexY++;
                        }
                        else
                        {
                            indexY = 0;
                            indexX = 0;
                        }
                        WriteAllMenu(indexY, indexX, choices, room: room);
                        break;
                    case ConsoleKey.UpArrow:
                        indexY = indexY - 1 >= 0 ? indexY - 1 : indexX == 0 ? _2dChoices.Count - 1 : _2dChoices.Count - 3;
                        WriteAllMenu(indexY, indexX,  choices, room: room);
                        break;
                    case ConsoleKey.Enter:
                        var choice = _2dChoices[indexY][indexX];
                        if (new[] { "Terug", "Ga door" }.Any(x => x == choice.Key))
                        {
                            Console.Clear();
                            return Tuple.Create(choice.Key, room.Seats.Intersect(choices.Select(x => new Seat { IsTaken = true, Row = x.Key[0].ToString(), Number = int.Parse(x.Key.Substring(1)) })).ToList());
                        }
                        if (!choices.Contains(choice) && !room.Seats.FirstOrDefault(x => $"{ x.Row}{ (x.Number < 10 ? "0" : "")}{ x.Number}" == choice.Key).IsTaken)
                            choices.Add(choice);
                        else
                            choices.Remove(choice);
                        WriteAllMenu(indexY, indexX, choices, room: room);
                        break;
                    case ConsoleKey.Backspace:
                        Console.Clear();
                        return Tuple.Create(_2dChoices[indexY][indexX].Key, room.Seats.Intersect(choices.Select(x => new Seat { IsTaken = true, Row = x.Key[0].ToString(), Number = int.Parse(x.Key.Substring(1)) })).ToList()); ;
                }
            }
        }
        
        private void WriteAllMenu(int indexY, int indexX, List<KeyValuePair<string, Action>> selectedChoices = null, Room room = null)
        {
            ConsoleHelper.WriteLogo(ConsoleColor.Red);
            ConsoleHelper.WriteBreadcrumb();

            if (!string.IsNullOrEmpty(_text))
                ConsoleHelper.ColorWriteLine(_text, _textColor);

            foreach (var row in _2dChoices)
            {
                foreach (var choice in row)
                {
                    bool seatIsTaken;
                    bool choiceIsSelected = selectedChoices != null && selectedChoices.Contains(choice);
                    if (new[] { "Terug", "Ga door" }.Any(x => x == choice.Key))
                    {
                        Console.WriteLine();
                        seatIsTaken = false;
                        ConsoleHelper.ColorWrite($"   {choice.Key}", choice.Key == _2dChoices[indexY].ElementAt(indexX).Key ? ConsoleColor.Red : ConsoleColor.White);
                    }
                    else
                    {
                        seatIsTaken = room.Seats.FirstOrDefault(x => $"{ x.Row}{ (x.Number < 10 ? "0" : "")}{ x.Number}" == choice.Key).IsTaken;

                        if (choice.Key == _2dChoices[indexY].ElementAt(indexX).Key)
                            ConsoleHelper.ColorWrite($"   [{choice.Key}]", choiceIsSelected && !seatIsTaken ? ConsoleColor.Yellow : ConsoleColor.Red);
                        else
                            ConsoleHelper.ColorWrite($"   [{choice.Key}]", choiceIsSelected && !seatIsTaken ? ConsoleColor.Green : seatIsTaken ? ConsoleColor.DarkRed : ConsoleColor.White);
                    }
                }
                Console.WriteLine();
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
                        ConsoleHelper.ColorWriteLine($"   {choice.Key}", ConsoleColor.Yellow);
                    else
                    {
                        if (choiceIsSelected)
                            ConsoleHelper.ColorWriteLine($"   {choice.Key}", ConsoleColor.Green);
                        else
                            Console.WriteLine($"   {choice.Key}");
                    }
                }
            }
        }
    }
}
