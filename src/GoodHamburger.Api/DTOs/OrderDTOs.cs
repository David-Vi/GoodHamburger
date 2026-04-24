using System.ComponentModel.DataAnnotations;

namespace GoodHamburger.Api.DTOs;

// ── Request DTOs ─────────────────────────────────────────────────────────────

/// <summary>Payload para criar ou atualizar um pedido.</summary>
public sealed class CreateOrderRequest
{
    /// <summary>IDs dos itens do cardápio. Máx. 3 (um por categoria).</summary>
    [Required]
    [MinLength(1)]
    [MaxLength(3)]
    public IList<int> MenuItemIds { get; init; } = [];
}

// ── Response DTOs ────────────────────────────────────────────────────────────

public sealed record OrderResponse(
    Guid Id,
    string Status,
    IReadOnlyList<OrderItemResponse> Items,
    decimal Subtotal,
    decimal DiscountRate,
    decimal DiscountAmount,
    decimal Total,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public sealed record OrderItemResponse(
    int MenuItemId,
    string Name,
    decimal UnitPrice,
    string Category
);

public sealed record MenuItemResponse(
    int Id,
    string Name,
    decimal Price,
    string Category
);

/// <summary>Envelope de erro padronizado — compatível com RFC 7807 Problem Details.</summary>
public sealed record ProblemResponse(
    string Type,
    string Title,
    int Status,
    string Detail,
    string? TraceId = null
);
