using System.Net.Mime;
using Challenge.Viceri.Superhero.Api.Commons;
using Challenge.Viceri.Superhero.Application.UseCases.SuperPowers.Commands;
using Challenge.Viceri.Superhero.Application.UseCases.SuperPowers.DataTransferObjects;
using Challenge.Viceri.Superhero.Application.UseCases.SuperPowers.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Challenge.Viceri.Superhero.Api.Controllers;

/// <summary>
/// Endpoint dos super-poderes que os hérois podem ter.
/// </summary>
/// <param name="mediator">Injeçao de dependência do mediator</param>
[Route("api/[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public sealed class SuperPowersController(ISender mediator) : ControllerBase
{
    /// <summary>
    /// Busca a lista de super-poderes cadastrados
    /// </summary>
    /// <returns>Lista de super-poderes cadastrados</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<SuperPowerDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<SuperPowerDto>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<SuperPowerDto>>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<SuperPowerDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllSuperPower()
    {
        var result = await mediator.Send(new GetAllSuperPowerQuery());

        if (!result.Any())
            return NotFound(ApiResult<IEnumerable<SuperPowerDto>>.FailureResult("Nenhum super-poder cadastrado", "NOT_FOUND"));

        return Ok(ApiResult<IEnumerable<SuperPowerDto>>.SuccessResult(result));
    }

    /// <summary>
    /// Busca um super-poder por id
    /// </summary>
    /// <param name="id">Id do super-poder a ser localizado</param>
    /// <returns>Super-poder procurado pelo id</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResult<SuperPowerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<SuperPowerDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult<SuperPowerDto>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResult<SuperPowerDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSuperPowerById(int id)
    {
        var result = await mediator.Send(new GetSuperPowerByIdQuery(id));

        if (result is null)
            return NotFound(ApiResult<SuperPowerDto>.FailureResult("Super-poder não encontrado", "NOT_FOUND"));

        return Ok(ApiResult<SuperPowerDto>.SuccessResult(result));
    }

    /// <summary>
    /// Cadastrar um super-poder
    /// </summary>
    /// <param name="command">Objeto contendo as informações para cadastro de um super-poder</param>
    /// <returns>Super-poder cadastrado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<SuperPowerDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResult<SuperPowerDto>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResult<SuperPowerDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateSuperPower([FromBody] CreateSuperPowerCommand command)
    {
        var result = await mediator.Send(command);

        return CreatedAtAction(
            actionName: nameof(GetSuperPowerById),
            routeValues: new { id = result?.Id },
            value: ApiResult<SuperPowerDto>.SuccessResult(result));
    }

    /// <summary>
    /// Atualiza informações de um super-poder
    /// </summary>
    /// <param name="id">Id do super-poder a ser atualizado</param>
    /// <param name="command">Objeto contendo as informações de atualização de um super-poder</param>
    /// <returns>Super-poder atualizado</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResult<SuperPowerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<SuperPowerDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult<SuperPowerDto>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResult<SuperPowerDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateSuperPower(int id, [FromBody] UpdateSuperPowerCommand command)
    {
        command.SetHeroId(id);
        var result = await mediator.Send(command);

        if (result is null)
            return NotFound(ApiResult<SuperPowerDto>.FailureResult("Herói não encontrado", "NOT_FOUND"));

        return Ok(ApiResult<SuperPowerDto>.SuccessResult(result));
    }

    /// <summary>
    /// Exclui um super-poder
    /// </summary>
    /// <param name="id">Id do super-poder a ser excluído</param>
    /// <returns>Super-poder excluído</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResult<SuperPowerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<SuperPowerDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult<SuperPowerDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteHero(int id)
    {
        var result = await mediator.Send(new DeleteSuperPowerCommand(id));

        if (result is null)
            return NotFound(ApiResult<SuperPowerDto>.FailureResult("Herói não encontrado", "NOT_FOUND"));

        return Ok(ApiResult<SuperPowerDto>.SuccessResult(result));
    }
}