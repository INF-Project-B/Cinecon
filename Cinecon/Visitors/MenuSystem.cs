using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinecon
{
    public static class MenuSystem
    {
        private static List<KeyValuePair<string, decimal>> _menuCart { get; set; } = new List<KeyValuePair<string, decimal>>();
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

        private static void ClearMenuCart(bool clearTickets = false)
        {
            if (clearTickets)
                _menuCart.Clear();
            else
                _menuCart = _menuCart.Where(x => x.Key.Contains("Ticket")).ToList();
        }
        
        public static void AddToCart(string name, decimal price)
            => _menuCart.Add(new KeyValuePair<string, decimal>(name, price));

        public static void ShowMenuConfirmation(DateTime date)
        {
            ConsoleHelper.LogoType = LogoType.Films;
            ConsoleHelper.Breadcrumb = null;

            var menuConfirmationChoice = ChoiceMenu.CreateConfirmationChoiceMenu("   Wil je het menu assortiment bekijken?\n").MakeChoice();

            if (menuConfirmationChoice.Key == "Ja")
                ShowMenu(date);
            else
            {
                ClearMenuCart();
                PaymentSystem.StartPaymentProcess(date);
            }
        }

        private static void ShowMenu(DateTime date)
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
                ReservationSystem.ShowSeats(date);
            else if (categoryChoice.Key == "Winkelmand legen")
            {
                ClearMenuCart();
                ShowMenu(date);
            }
            else if (categoryChoice.Key == "Ga door")
                PaymentSystem.StartPaymentProcess(date);
            else
                ShowCategoryItems(JsonHelper.Menu.FirstOrDefault(x => x.Name == categoryChoice.Key), date);
        }

        private static void ShowCategoryItems(MenuCategory category, DateTime date)
        {
            ConsoleHelper.LogoType = LogoType.Menu;
            ConsoleHelper.Breadcrumb = $"Categorie: {category.Name}";

            var itemChoices = new Dictionary<string, Action>();

            foreach (var item in category.MenuItems)
                itemChoices[item.Name] = null;

            var itemChoiceMenu = new ChoiceMenu(itemChoices, true, MenuCartText);

            var itemChoice = itemChoiceMenu.MakeChoice();

            if (itemChoice.Key == "Terug")
                ShowMenu(date);
            else
                ShowItemTypes(category, itemChoice.Key, date);
        }

        private static void ShowItemTypes(MenuCategory category, string menuItem, DateTime date)
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
                ShowCategoryItems(category, date);
            else
            {
                var itemTypeData = itemTypes.FirstOrDefault(x => typeChoice.Key.Contains(x.Key));
                AddToCart($"{menuItem} {itemTypeData.Key.ToLower()} - {itemTypeData.Value:0.00}", itemTypeData.Value);
                ShowMenu(date);
            }
        }
    }
}
