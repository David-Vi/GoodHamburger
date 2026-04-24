using System.Net.Http.Json;

namespace GoodHamburger.Blazor.Services;

// ── DTOs espelhados do backend ───────────────────────────────────────────────

public record MenuItemDto(int Id, string Name, decimal Price, string Category);

public record OrderItemDto(int MenuItemId, string Name, decimal UnitPrice, string Category);

public record OrderDto(
    Guid Id,
    string Status,
    List<OrderItemDto> Items,
    decimal Subtotal,
    decimal DiscountRate,
    decimal DiscountAmount,
    decimal Total,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateOrderRequest(List<int> MenuItemIds);

// ── Interface ────────────────────────────────────────────────────────────────

public interface IApiService
{
    Task<List<MenuItemDto>> GetMenuAsync();
    Task<List<OrderDto>> GetOrdersAsync();
    Task<OrderDto> GetOrderByIdAsync(Guid id);
    Task<OrderDto> CreateOrderAsync(List<int> menuItemIds);
    Task<OrderDto> UpdateOrderAsync(Guid id, List<int> menuItemIds);
    Task DeleteOrderAsync(Guid id);
}

// ── Implementação ─────────────────────────────────────────────────────────────

public sealed class ApiService(HttpClient http) : IApiService
{
    public Task<List<MenuItemDto>> GetMenuAsync() =>
        http.GetFromJsonAsync<List<MenuItemDto>>("api/v1/menu")!;

    public Task<List<OrderDto>> GetOrdersAsync() =>
        http.GetFromJsonAsync<List<OrderDto>>("api/v1/orders")!;

    public Task<OrderDto> GetOrderByIdAsync(Guid id) =>
        http.GetFromJsonAsync<OrderDto>($"api/v1/orders/{id}")!;

    public async Task<OrderDto> CreateOrderAsync(List<int> menuItemIds)
    {
        var response = await http.PostAsJsonAsync("api/v1/orders", new CreateOrderRequest(menuItemIds));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<OrderDto>())!;
    }

    public async Task<OrderDto> UpdateOrderAsync(Guid id, List<int> menuItemIds)
    {
        var response = await http.PutAsJsonAsync($"api/v1/orders/{id}", new CreateOrderRequest(menuItemIds));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<OrderDto>())!;
    }

    public async Task DeleteOrderAsync(Guid id)
    {
        var response = await http.DeleteAsync($"api/v1/orders/{id}");
        response.EnsureSuccessStatusCode();
    }
}
