using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinecon
{
    public static class MenuSystem
    {
        private static readonly List<KeyValuePair<string, decimal>> _menuCart = new List<KeyValuePair<string, decimal>>();
        public static string MenuCartText
        {
            get
            {
                var menuCart = new List<KeyValuePair<string, decimal>>();
                foreach (var item in _menuCart)
                {
                    var count = _menuCart.Count(x => x.Key == item.Key);
                    menuCart.Add(count > 1 ? new KeyValuePair<string, decimal>(item.Key + $" (x{count})", item.Value) : item);
                }
                return $"   Winkelmand (totaal: {_menuCart.Select(x => x.Value).Sum():0.00} euro)\n     {(menuCart.Any() ? string.Join("\n     ", menuCart.ToHashSet().Select(x => x.Key)) : "Leeg")}\n";
            }
        }

        public static void ShowMenuConfirmation()
        {
            ConsoleHelper.LogoType = LogoType.Films;
            ConsoleHelper.Breadcrumb = null;

            var menuConfirmationChoice = ChoiceMenu.CreateConfirmationChoiceMenu("   Wil je het menu assortiment bekijken?\n").MakeChoice();

            if (menuConfirmationChoice.Key == "Ja")
                ShowMenu();
            else
                PaymentSystem.StartPaymentProcess();
        }

        private static void ShowMenu()
        {
            ConsoleHelper.LogoType = LogoType.Menu;
            ConsoleHelper.Breadcrumb = "Films / Titel / Koop tickets / Menu";

            var categoryChoices = new Dictionary<string, Action>();

            foreach (var category in JsonHelper.Menu)
                categoryChoices[category.Name] = null;

            categoryChoices["Winkelmand legen"] = null;
            categoryChoices["Ga door"] = null;

            var categoryChoiceMenu = new ChoiceMenu(categoryChoices, true, MenuCartText);

            var categoryChoice = categoryChoiceMenu.MakeChoice();

            if (categoryChoice.Key == "Terug")
                ShowMenuConfirmation();
            else if (categoryChoice.Key == "Winkelmand legen")
            {
                _menuCart.Clear();
                ShowMenu();
            }
            else if (categoryChoice.Key == "Ga door")
                PaymentSystem.StartPaymentProcess();
            else
                ShowCategoryItems(JsonHelper.Menu.FirstOrDefault(x => x.Name == categoryChoice.Key));
        }

        private static void ShowCategoryItems(MenuCategory category)
        {
            ConsoleHelper.LogoType = LogoType.Menu;
            ConsoleHelper.Breadcrumb = $"Categorie: {category.Name}";

            var itemChoices = new Dictionary<string, Action>();

            foreach (var item in category.MenuItems)
                itemChoices[item.Name] = null;

            var itemChoiceMenu = new ChoiceMenu(itemChoices, true, MenuCartText);

            var itemChoice = itemChoiceMenu.MakeChoice();

            if (itemChoice.Key == "Terug")
                ShowMenu();
            else
                ShowItemTypes(category, itemChoice.Key);
        }

        private static void ShowItemTypes(MenuCategory category, string menuItem)
        {
            ConsoleHelper.LogoType = LogoType.Menu;
            ConsoleHelper.Breadcrumb += $"\n   Product: {menuItem}";

            var itemTypes = category.MenuItems.FirstOrDefault(x => x.Name == menuItem).ItemTypes;

            var typeChoices = new Dictionary<string, Action>();

            foreach (var type in itemTypes)
                typeChoices[$"{type.Key} - {type.Value:0.00} euro"] = null;

            var typeChoiceMenu = new ChoiceMenu(typeChoices, true, MenuCartText);

            var typeChoice = typeChoiceMenu.MakeChoice();

            if (typeChoice.Key == "Terug")
                ShowCategoryItems(category);
            else
            {
                var itemTypeData = itemTypes.FirstOrDefault(x => typeChoice.Key.Contains(x.Key));
                _menuCart.Add(new KeyValuePair<string, decimal>($"{menuItem} {itemTypeData.Key.ToLower()} - {itemTypeData.Value:0.00}", itemTypeData.Value));
                ShowMenu();
            }
        }
    }
}
