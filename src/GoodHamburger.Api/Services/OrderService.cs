using GoodHamburger.Api.DTOs;
using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Exceptions;
using GoodHamburger.Core.Interfaces;

namespace GoodHamburger.Api.Services;

public interface IOrderService
{
    Task<IReadOnlyList<OrderResponse>> GetAllAsync(CancellationToken ct = default);
    Task<OrderResponse> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<OrderResponse> CreateAsync(CreateOrderRequest request, CancellationToken ct = default);
    Task<OrderResponse> UpdateAsync(Guid id, CreateOrderRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public sealed class OrderService(IOrderRepository orderRepo, IMenuRepository menuRepo) : IOrderService
{
    public async Task<IReadOnlyList<OrderResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var orders = await orderRepo.GetAllAsync(ct);
        return orders.Select(MapToResponse).ToList();
    }

    public async Task<OrderResponse> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var order = await orderRepo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Order), id);
        return MapToResponse(order);
    }

    public async Task<OrderResponse> CreateAsync(CreateOrderRequest request, CancellationToken ct = default)
    {
        ValidateRequestNotEmpty(request);

        var order = new Order();
        var menuItems = await ResolveMenuItemsAsync(request.MenuItemIds, ct);

        foreach (var item in menuItems)
            order.AddItem(item); // lança DomainException se duplicado

        await orderRepo.AddAsync(order, ct);
        return MapToResponse(order);
    }

    public async Task<OrderResponse> UpdateAsync(Guid id, CreateOrderRequest request, CancellationToken ct = default)
    {
        var order = await orderRepo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Order), id);

        ValidateRequestNotEmpty(request);

        var menuItems = await ResolveMenuItemsAsync(request.MenuItemIds, ct);
        order.ReplaceItems(menuItems);

        await orderRepo.UpdateAsync(order, ct);
        return MapToResponse(order);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var exists = await orderRepo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Order), id);

        await orderRepo.DeleteAsync(id, ct);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static void ValidateRequestNotEmpty(CreateOrderRequest request)
    {
        if (request.MenuItemIds is null || request.MenuItemIds.Count == 0)
            throw new DomainException("O pedido deve conter pelo menos um item.");

        if (request.MenuItemIds.Count > 3)
            throw new DomainException("Um pedido aceita no máximo 3 itens (um por categoria).");
    }

    private async Task<List<Core.Entities.MenuItem>> ResolveMenuItemsAsync(
        IList<int> ids, CancellationToken ct)
    {
        var result = new List<Core.Entities.MenuItem>();
        foreach (var id in ids)
        {
            var item = await menuRepo.GetByIdAsync(id, ct)
                ?? throw new NotFoundException("MenuItem", id);
            result.Add(item);
        }
        return result;
    }

    private static OrderResponse MapToResponse(Order order) => new(
        order.Id,
        order.Status.ToString(),
        order.Items.Select(i => new OrderItemResponse(
            i.MenuItemId, i.Name, i.UnitPrice, i.Category.ToString()
        )).ToList(),
        order.Subtotal,
        order.DiscountRate,
        order.DiscountAmount,
        order.Total,
        order.CreatedAt,
        order.UpdatedAt
    );
}
