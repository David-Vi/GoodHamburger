using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GoodHamburger.Api.DTOs;
using GoodHamburger.Api.Services;

namespace GoodHamburger.Api.Controllers;

/// <summary>
/// CRUD completo de pedidos.
/// Todos os endpoints retornam Problem Details em caso de erro.
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/orders")]
[Produces("application/json")]
public sealed class OrdersController(IOrderService orderService) : ControllerBase
{
    /// <summary>Lista todos os pedidos, do mais recente ao mais antigo.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<OrderResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var orders = await orderService.GetAllAsync(ct);
        return Ok(orders);
    }

    /// <summary>Retorna um pedido pelo seu identificador único (GUID).</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<OrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var order = await orderService.GetByIdAsync(id, ct);
        return Ok(order);
    }

    /// <summary>
    /// Cria um novo pedido.
    /// Regras:
    ///   • Máximo 3 itens (um sanduíche, uma batata, um refrigerante).
    ///   • Itens duplicados na mesma categoria retornam 422.
    ///   • Desconto calculado automaticamente.
    /// </summary>
    [HttpPost]
    [ProducesResponseType<OrderResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemResponse>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var created = await orderService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Atualiza os itens de um pedido existente.
    /// Substitui completamente os itens anteriores.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType<OrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemResponse>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateOrderRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var updated = await orderService.UpdateAsync(id, request, ct);
        return Ok(updated);
    }

    /// <summary>Remove um pedido permanentemente.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await orderService.DeleteAsync(id, ct);
        return NoContent();
    }
}
