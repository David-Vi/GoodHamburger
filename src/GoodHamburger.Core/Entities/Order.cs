using GoodHamburger.Core.Enums;
using GoodHamburger.Core.Exceptions;

namespace GoodHamburger.Core.Entities;

/// <summary>
/// Agregado raiz do pedido. Encapsula as regras de negócio de desconto
/// e restrições de itens — o lugar certo para lógica de domínio.
/// </summary>
public sealed class Order
{
    private readonly List<OrderItem> _items = new();

    public Guid Id { get; private set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
    public OrderStatus Status { get; private set; } = OrderStatus.Open;

    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    // ── Computed properties ──────────────────────────────────────────────────
    public decimal Subtotal => _items.Sum(i => i.UnitPrice);

    public decimal DiscountRate => CalculateDiscountRate();

    public decimal DiscountAmount => Math.Round(Subtotal * DiscountRate, 2, MidpointRounding.AwayFromZero);

    public decimal Total => Subtotal - DiscountAmount;

    // ── Mutations ────────────────────────────────────────────────────────────

    /// <summary>Adiciona item ao pedido, aplicando todas as validações de domínio.</summary>
    public void AddItem(MenuItem menuItem)
    {
        ValidateDuplicate(menuItem);
        _items.Add(new OrderItem(menuItem.Id, menuItem.Name, menuItem.Price, menuItem.Category));
        Touch();
    }

    /// <summary>Substitui todos os itens do pedido (usado no UPDATE).</summary>
    public void ReplaceItems(IEnumerable<MenuItem> newItems)
    {
        _items.Clear();
        foreach (var item in newItems)
            AddItem(item); // reusar validação
        Touch();
    }

    public void Close()
    {
        Status = OrderStatus.Closed;
        Touch();
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    private void ValidateDuplicate(MenuItem incoming)
    {
        bool alreadyHasCategory = _items.Any(i => i.Category == incoming.Category);
        if (alreadyHasCategory)
        {
            throw new DomainException(
                $"O pedido já contém um item da categoria '{CategoryLabel(incoming.Category)}'. " +
                "Cada pedido aceita no máximo um item por categoria.");
        }
    }

    /// <summary>
    /// Regras de desconto — tabela de prioridade:
    ///   Sanduíche + Batata + Refrigerante → 20 %
    ///   Sanduíche + Refrigerante          → 15 %
    ///   Sanduíche + Batata                → 10 %
    ///   Qualquer outra combinação         →  0 %
    /// </summary>
    private decimal CalculateDiscountRate()
    {
        bool hasSandwich = _items.Any(i => i.Category == MenuItemCategory.Sandwich);
        bool hasSide     = _items.Any(i => i.Category == MenuItemCategory.SideDish);
        bool hasDrink    = _items.Any(i => i.Category == MenuItemCategory.Drink);

        return (hasSandwich, hasSide, hasDrink) switch
        {
            (true, true, true)   => 0.20m,
            (true, false, true)  => 0.15m,
            (true, true, false)  => 0.10m,
            _                    => 0.00m
        };
    }

    private static string CategoryLabel(MenuItemCategory cat) => cat switch
    {
        MenuItemCategory.Sandwich => "Sanduíche",
        MenuItemCategory.SideDish => "Acompanhamento",
        MenuItemCategory.Drink    => "Bebida",
        _                         => cat.ToString()
    };

    private void Touch() => UpdatedAt = DateTime.UtcNow;
}

public enum OrderStatus { Open, Closed }

/// <summary>
/// Snapshot do item no momento em que foi adicionado ao pedido.
/// Preserva o preço histórico mesmo se o catálogo mudar.
/// </summary>
public sealed record OrderItem(int MenuItemId, string Name, decimal UnitPrice, MenuItemCategory Category);
