using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GoodHamburger.Api.DTOs;
using GoodHamburger.Api.Services;

namespace GoodHamburger.Api.Controllers;

/// <summary>
/// Endpoint de consulta ao cardápio — somente leitura.
/// </summary>
[ApiController]
[AllowAnonymous]
[Route("api/v1/menu")]
[Produces("application/json")]
public sealed class MenuController(IMenuService menuService) : ControllerBase
{
    /// <summary>Retorna todos os itens do cardápio com preço e categoria.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<MenuItemResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMenu(CancellationToken ct)
    {
        var menu = await menuService.GetMenuAsync(ct);
        return Ok(menu);
    }
}
