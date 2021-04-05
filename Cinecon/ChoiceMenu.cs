using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinecon
{
    public class ChoiceMenu
    {
        private readonly Dictionary<string, Action> _choices;

        public ChoiceMenu(Dictionary<string, Action> choices)
        {
            _choices = choices;
        }

        public KeyValuePair<string, Action> MakeChoice()
        {
            while (true)
            {
                Console.WriteLine("\n");

                // Write all choices in the console.
                for (int i = 0; i < _choices.Count; i++)
                    Console.WriteLine($"[{i + 1}] {_choices.ElementAt(i).Key}");

                Console.WriteLine("\n");

                var choiceRead = Console.ReadLine();

                KeyValuePair<string, Action> choice;

                // If user typed number.
                if (int.TryParse(choiceRead, out int choiceNumber))
                {
                    if (choiceNumber > _choices.Count || choiceNumber < 1)
                    {
                        Console.Clear();
                        Console.WriteLine("Invalid number!");
                        continue;
                    }

                    choice = _choices.ElementAt(choiceNumber - 1);
                }
                // If user typed string.
                else
                {
                    if (_choices.Keys.Any(x => x.ToLower() == choiceRead.ToLower()))
                        choice = _choices.FirstOrDefault(x => x.Key.ToLower() == choiceRead.ToLower());
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Invalid string!");
                        continue;
                    }
                }

                return choice;
            }
        }
    }
}
