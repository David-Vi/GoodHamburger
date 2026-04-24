using GoodHamburger.Core.Enums;

namespace GoodHamburger.Core.Entities;

/// <summary>
/// Item do cardápio — imutável por design (registro de catálogo).
/// </summary>
public sealed class MenuItem
{
    public int Id { get; init; }
    public string Name { get; init; } = default!;
    public decimal Price { get; init; }
    public MenuItemCategory Category { get; init; }

    // Seed data centralizada aqui para ser usada tanto no InMemory quanto em migrations futuras.
    public static readonly IReadOnlyList<MenuItem> Catalog = new List<MenuItem>
    {
        new() { Id = (int)MenuItemId.XBurger,    Name = "X Burger",     Price = 5.00m, Category = MenuItemCategory.Sandwich },
        new() { Id = (int)MenuItemId.XEgg,       Name = "X Egg",        Price = 4.50m, Category = MenuItemCategory.Sandwich },
        new() { Id = (int)MenuItemId.XBacon,     Name = "X Bacon",      Price = 7.00m, Category = MenuItemCategory.Sandwich },
        new() { Id = (int)MenuItemId.FrenchFries, Name = "Batata Frita", Price = 2.00m, Category = MenuItemCategory.SideDish },
        new() { Id = (int)MenuItemId.Soda,       Name = "Refrigerante", Price = 2.50m, Category = MenuItemCategory.Drink   },
    };
}
