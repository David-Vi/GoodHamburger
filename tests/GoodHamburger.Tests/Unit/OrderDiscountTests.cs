using FluentAssertions;
using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Enums;
using GoodHamburger.Core.Exceptions;
using Xunit;

namespace GoodHamburger.Tests.Unit;

/// <summary>
/// Testes unitários das regras de negócio do agregado Order.
/// Cobrem todas as combinações de desconto e validações de domínio.
/// </summary>
public sealed class OrderDiscountTests
{
    // ── Fixtures ─────────────────────────────────────────────────────────────
    private static readonly MenuItem XBurger    = new() { Id = 1, Name = "X Burger",    Price = 5.00m, Category = MenuItemCategory.Sandwich };
    private static readonly MenuItem XEgg       = new() { Id = 2, Name = "X Egg",       Price = 4.50m, Category = MenuItemCategory.Sandwich };
    private static readonly MenuItem XBacon     = new() { Id = 3, Name = "X Bacon",     Price = 7.00m, Category = MenuItemCategory.Sandwich };
    private static readonly MenuItem FrenchFries= new() { Id = 4, Name = "Batata Frita",Price = 2.00m, Category = MenuItemCategory.SideDish };
    private static readonly MenuItem Soda       = new() { Id = 5, Name = "Refrigerante",Price = 2.50m, Category = MenuItemCategory.Drink   };

    // ── Desconto 20% — Sanduíche + Batata + Refrigerante ─────────────────────

    [Fact]
    public void Order_WithSandwichSideAndDrink_ShouldApply20PercentDiscount()
    {
        var order = new Order();
        order.AddItem(XBurger);
        order.AddItem(FrenchFries);
        order.AddItem(Soda);

        order.Subtotal.Should().Be(9.50m);         // 5.00 + 2.00 + 2.50
        order.DiscountRate.Should().Be(0.20m);
        order.DiscountAmount.Should().Be(1.90m);   // 9.50 * 20%
        order.Total.Should().Be(7.60m);
    }

    [Fact]
    public void Order_WithXBaconSideAndDrink_ShouldApply20PercentDiscount()
    {
        var order = new Order();
        order.AddItem(XBacon);
        order.AddItem(FrenchFries);
        order.AddItem(Soda);

        order.Subtotal.Should().Be(11.50m);        // 7.00 + 2.00 + 2.50
        order.DiscountRate.Should().Be(0.20m);
        order.DiscountAmount.Should().Be(2.30m);
        order.Total.Should().Be(9.20m);
    }

    // ── Desconto 15% — Sanduíche + Refrigerante ──────────────────────────────

    [Fact]
    public void Order_WithSandwichAndDrinkOnly_ShouldApply15PercentDiscount()
    {
        var order = new Order();
        order.AddItem(XBurger);
        order.AddItem(Soda);

        order.Subtotal.Should().Be(7.50m);         // 5.00 + 2.50
        order.DiscountRate.Should().Be(0.15m);
        order.DiscountAmount.Should().Be(1.13m);   // 7.50 * 15% = 1.125 → arredondado
        order.Total.Should().Be(6.37m);
    }

    [Fact]
    public void Order_WithXEggAndDrink_ShouldApply15PercentDiscount()
    {
        var order = new Order();
        order.AddItem(XEgg);
        order.AddItem(Soda);

        order.Subtotal.Should().Be(7.00m);
        order.DiscountRate.Should().Be(0.15m);
        order.DiscountAmount.Should().Be(1.05m);
        order.Total.Should().Be(5.95m);
    }

    // ── Desconto 10% — Sanduíche + Batata ────────────────────────────────────

    [Fact]
    public void Order_WithSandwichAndSideOnly_ShouldApply10PercentDiscount()
    {
        var order = new Order();
        order.AddItem(XBurger);
        order.AddItem(FrenchFries);

        order.Subtotal.Should().Be(7.00m);         // 5.00 + 2.00
        order.DiscountRate.Should().Be(0.10m);
        order.DiscountAmount.Should().Be(0.70m);
        order.Total.Should().Be(6.30m);
    }

    // ── Sem desconto ─────────────────────────────────────────────────────────

    [Fact]
    public void Order_WithSandwichOnly_ShouldHaveNoDiscount()
    {
        var order = new Order();
        order.AddItem(XBurger);

        order.DiscountRate.Should().Be(0.00m);
        order.DiscountAmount.Should().Be(0.00m);
        order.Total.Should().Be(order.Subtotal);
    }

    [Fact]
    public void Order_WithSideAndDrinkNoSandwich_ShouldHaveNoDiscount()
    {
        var order = new Order();
        order.AddItem(FrenchFries);
        order.AddItem(Soda);

        order.DiscountRate.Should().Be(0.00m);
        order.Total.Should().Be(4.50m);
    }

    [Fact]
    public void Order_WithDrinkOnly_ShouldHaveNoDiscount()
    {
        var order = new Order();
        order.AddItem(Soda);

        order.DiscountRate.Should().Be(0.00m);
        order.Total.Should().Be(2.50m);
    }

    // ── Validação de duplicatas ───────────────────────────────────────────────

    [Fact]
    public void Order_AddingDuplicateSandwich_ShouldThrowDomainException()
    {
        var order = new Order();
        order.AddItem(XBurger);

        Action act = () => order.AddItem(XEgg); // Segundo sanduíche

        act.Should().Throw<DomainException>()
           .WithMessage("*sanduíche*");
    }

    [Fact]
    public void Order_AddingDuplicateSide_ShouldThrowDomainException()
    {
        var order = new Order();
        order.AddItem(FrenchFries);

        Action act = () => order.AddItem(FrenchFries);

        act.Should().Throw<DomainException>()
           .WithMessage("*Acompanhamento*");
    }

    [Fact]
    public void Order_AddingDuplicateDrink_ShouldThrowDomainException()
    {
        var order = new Order();
        order.AddItem(Soda);

        Action act = () => order.AddItem(Soda);

        act.Should().Throw<DomainException>()
           .WithMessage("*Bebida*");
    }

    [Fact]
    public void Order_AddingSameItemTwice_ShouldThrowDomainException()
    {
        var order = new Order();
        order.AddItem(XBurger);

        Action act = () => order.AddItem(XBurger);

        act.Should().Throw<DomainException>();
    }

    // ── ReplaceItems ─────────────────────────────────────────────────────────

    [Fact]
    public void Order_ReplaceItems_ShouldRecalculateDiscountCorrectly()
    {
        var order = new Order();
        order.AddItem(XBurger);         // Inicialmente sem desconto
        order.DiscountRate.Should().Be(0.00m);

        order.ReplaceItems(new[] { XBacon, FrenchFries, Soda }); // Combo completo

        order.DiscountRate.Should().Be(0.20m);
        order.Items.Should().HaveCount(3);
    }

    // ── Subtotal / Total sanity ───────────────────────────────────────────────

    [Fact]
    public void Order_Total_ShouldNeverBeNegative()
    {
        var order = new Order();
        order.AddItem(XBurger);
        order.AddItem(FrenchFries);
        order.AddItem(Soda);

        order.Total.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Order_TotalPlusDiscount_ShouldEqualSubtotal()
    {
        var order = new Order();
        order.AddItem(XEgg);
        order.AddItem(FrenchFries);
        order.AddItem(Soda);

        (order.Total + order.DiscountAmount).Should().BeApproximately(order.Subtotal, 0.01m);
    }
}
