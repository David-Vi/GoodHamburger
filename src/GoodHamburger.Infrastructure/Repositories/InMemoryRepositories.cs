using System.Collections.Concurrent;
using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Interfaces;

namespace GoodHamburger.Infrastructure.Repositories;

/// <summary>
/// Repositório em memória — thread-safe via ConcurrentDictionary.
/// Substituível por EF Core sem alterar nenhuma linha da camada de serviço.
/// </summary>
public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, Order> _store = new();

    public Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default)
    {
        IReadOnlyList<Order> list = _store.Values
            .OrderByDescending(o => o.CreatedAt)
            .ToList();
        return Task.FromResult(list);
    }

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        Task.FromResult(_store.TryGetValue(id, out var order) ? order : null);

    public Task AddAsync(Order order, CancellationToken ct = default)
    {
        _store[order.Id] = order;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Order order, CancellationToken ct = default)
    {
        _store[order.Id] = order;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        _store.TryRemove(id, out _);
        return Task.CompletedTask;
    }
}

/// <summary>
/// Catálogo em memória — dados imutáveis do domínio.
/// </summary>
public sealed class InMemoryMenuRepository : IMenuRepository
{
    private static readonly IReadOnlyList<MenuItem> _catalog = MenuItem.Catalog;

    public Task<IReadOnlyList<MenuItem>> GetAllAsync(CancellationToken ct = default) =>
        Task.FromResult(_catalog);

    public Task<MenuItem?> GetByIdAsync(int id, CancellationToken ct = default) =>
        Task.FromResult(_catalog.FirstOrDefault(m => m.Id == id));
}
