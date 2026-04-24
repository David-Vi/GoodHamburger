using GoodHamburger.Api.DTOs;
using GoodHamburger.Core.Interfaces;

namespace GoodHamburger.Api.Services;

public interface IMenuService
{
    Task<IReadOnlyList<MenuItemResponse>> GetMenuAsync(CancellationToken ct = default);
}

public sealed class MenuService(IMenuRepository menuRepo) : IMenuService
{
    public async Task<IReadOnlyList<MenuItemResponse>> GetMenuAsync(CancellationToken ct = default)
    {
        var items = await menuRepo.GetAllAsync(ct);
        return items.Select(i => new MenuItemResponse(
            i.Id, i.Name, i.Price, i.Category.ToString()
        )).ToList();
    }
}
