using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using GoodHamburger.Api.Auth;
using GoodHamburger.Api.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace GoodHamburger.Tests.Integration;

/// <summary>
/// Testes de integração dos endpoints REST usando WebApplicationFactory.
/// Testam o pipeline completo: routing → controller → service → repositório.
/// </summary>
public sealed class OrdersApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string TestApiKey = "goodhamburger-local-dev-key";
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public OrdersApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Add(ApiKeyAuthenticationDefaults.HeaderName, TestApiKey);
    }

    // ── GET /api/v1/menu ──────────────────────────────────────────────────────

    [Fact]
    public async Task GetMenu_ShouldReturn5Items()
    {
        var response = await _client.GetAsync("/api/v1/menu");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content.ReadFromJsonAsync<List<MenuItemResponse>>(_jsonOptions);
        items.Should().HaveCount(5);
        items!.Select(i => i.Name).Should().Contain(["X Burger", "X Egg", "X Bacon", "Batata Frita", "Refrigerante"]);
    }

    // ── POST /api/v1/orders ───────────────────────────────────────────────────

    [Fact]
    public async Task CreateOrder_WithValidCombo_ShouldReturn201WithDiscount()
    {
        var request = Request(1, 4, 5); // X Burger + Batata + Soda → 20%

        var response = await _client.PostAsJsonAsync("/api/v1/orders", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var order = await response.Content.ReadFromJsonAsync<OrderResponse>(_jsonOptions);
        order.Should().NotBeNull();
        order!.DiscountRate.Should().Be(0.20m);
        order.Subtotal.Should().Be(9.50m);
        order.Total.Should().Be(7.60m);
        order.Items.Should().HaveCount(3);
    }

    [Fact]
    public async Task CreateOrder_WithoutApiKey_ShouldReturn401()
    {
        using var clientWithoutApiKey = new WebApplicationFactory<Program>().CreateClient();
        var response = await clientWithoutApiKey.PostAsJsonAsync("/api/v1/orders", Request(1, 4, 5));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateOrder_SandwichAndDrink_ShouldApply15Percent()
    {
        var request = Request(1, 5); // X Burger + Soda

        var response = await _client.PostAsJsonAsync("/api/v1/orders", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var order = await response.Content.ReadFromJsonAsync<OrderResponse>(_jsonOptions);
        order!.DiscountRate.Should().Be(0.15m);
    }

    [Fact]
    public async Task CreateOrder_SandwichAndSide_ShouldApply10Percent()
    {
        var request = Request(1, 4); // X Burger + Batata

        var response = await _client.PostAsJsonAsync("/api/v1/orders", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var order = await response.Content.ReadFromJsonAsync<OrderResponse>(_jsonOptions);
        order!.DiscountRate.Should().Be(0.10m);
    }

    [Fact]
    public async Task CreateOrder_WithDuplicateCategory_ShouldReturn422()
    {
        var request = Request(1, 2); // X Burger + X Egg (2 sanduíches)

        var response = await _client.PostAsJsonAsync("/api/v1/orders", request);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task CreateOrder_WithInvalidMenuItemId_ShouldReturn404()
    {
        var request = Request(999); // Item inexistente

        var response = await _client.PostAsJsonAsync("/api/v1/orders", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateOrder_WithEmptyItems_ShouldReturn422()
    {
        var request = Request();

        var response = await _client.PostAsJsonAsync("/api/v1/orders", request);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // ── GET /api/v1/orders ────────────────────────────────────────────────────

    [Fact]
    public async Task GetAll_ShouldReturnListOfOrders()
    {
        // Cria um pedido primeiro
        await _client.PostAsJsonAsync("/api/v1/orders", Request(1));

        var response = await _client.GetAsync("/api/v1/orders");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var orders = await response.Content.ReadFromJsonAsync<List<OrderResponse>>(_jsonOptions);
        orders.Should().NotBeEmpty();
    }

    // ── GET /api/v1/orders/{id} ───────────────────────────────────────────────

    [Fact]
    public async Task GetById_WithValidId_ShouldReturnOrder()
    {
        var created = await CreateOrderAndGetResponse(Request(3, 4, 5));

        var response = await _client.GetAsync($"/api/v1/orders/{created!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var order = await response.Content.ReadFromJsonAsync<OrderResponse>(_jsonOptions);
        order!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldReturn404()
    {
        var fakeId = Guid.NewGuid();
        var response = await _client.GetAsync($"/api/v1/orders/{fakeId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── PUT /api/v1/orders/{id} ───────────────────────────────────────────────

    [Fact]
    public async Task UpdateOrder_ShouldRecalculateDiscount()
    {
        // Cria com apenas sanduíche
        var created = await CreateOrderAndGetResponse(Request(1));
        created!.DiscountRate.Should().Be(0.00m);

        // Atualiza para combo completo
        var updateRequest = Request(2, 4, 5);
        var response = await _client.PutAsJsonAsync($"/api/v1/orders/{created.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<OrderResponse>(_jsonOptions);
        updated!.DiscountRate.Should().Be(0.20m);
    }

    [Fact]
    public async Task UpdateOrder_WithInvalidId_ShouldReturn404()
    {
        var response = await _client.PutAsJsonAsync(
            $"/api/v1/orders/{Guid.NewGuid()}",
            Request(1));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── DELETE /api/v1/orders/{id} ────────────────────────────────────────────

    [Fact]
    public async Task DeleteOrder_WithValidId_ShouldReturn204()
    {
        var created = await CreateOrderAndGetResponse(Request(1));

        var response = await _client.DeleteAsync($"/api/v1/orders/{created!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteOrder_ThenGet_ShouldReturn404()
    {
        var created = await CreateOrderAndGetResponse(Request(1));
        await _client.DeleteAsync($"/api/v1/orders/{created!.Id}");

        var response = await _client.GetAsync($"/api/v1/orders/{created.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteOrder_WithInvalidId_ShouldReturn404()
    {
        var response = await _client.DeleteAsync($"/api/v1/orders/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── Security headers ──────────────────────────────────────────────────────

    [Fact]
    public async Task AnyEndpoint_ShouldReturnSecurityHeaders()
    {
        var response = await _client.GetAsync("/api/v1/menu");

        response.Headers.Should().ContainKey("X-Content-Type-Options");
        response.Headers.Should().ContainKey("X-Frame-Options");
        response.Headers.Should().ContainKey("Content-Security-Policy");
    }

    // ── Helper ────────────────────────────────────────────────────────────────

    private async Task<OrderResponse?> CreateOrderAndGetResponse(CreateOrderRequest request)
    {
        var response = await _client.PostAsJsonAsync("/api/v1/orders", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OrderResponse>(_jsonOptions);
    }

    private static CreateOrderRequest Request(params int[] menuItemIds) => new()
    {
        MenuItemIds = menuItemIds.ToList()
    };
}
