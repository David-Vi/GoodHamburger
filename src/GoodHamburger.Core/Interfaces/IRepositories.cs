using GoodHamburger.Core.Entities;

namespace GoodHamburger.Core.Interfaces;

public interface IOrderRepository
{
    Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Order order, CancellationToken ct = default);
    Task UpdateAsync(Order order, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IMenuRepository
{
    Task<IReadOnlyList<MenuItem>> GetAllAsync(CancellationToken ct = default);
    Task<MenuItem?> GetByIdAsync(int id, CancellationToken ct = default);
}
