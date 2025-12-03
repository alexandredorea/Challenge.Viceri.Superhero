using System.Net.Mime;
using Challenge.Viceri.Superhero.Api.Commons;
using Challenge.Viceri.Superhero.Application.UseCases.SuperPowers.Commands;
using Challenge.Viceri.Superhero.Application.UseCases.SuperPowers.DataTransferObjects;
using Challenge.Viceri.Superhero.Application.UseCases.SuperPowers.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Challenge.Viceri.Superhero.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public sealed class SuperPowersController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<SuperPowerDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<SuperPowerDto>>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllSuperPower()
    {
        var result = await mediator.Send(new GetAllSuperPowerQuery());

        if (result?.Count() == 0)
            return NotFound(ApiResult<IEnumerable<SuperPowerDto>>.FailureResult("Nenhum super-poder cadastrado", "NOT_FOUND"));

        return Ok(ApiResult<IEnumerable<SuperPowerDto>>.SuccessResult(result));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResult<SuperPowerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<SuperPowerDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSuperPowerById(int id)
    {
        var result = await mediator.Send(new GetSuperPowerByIdQuery(id));

        if (result is null)
            return NotFound(ApiResult<SuperPowerDto>.FailureResult("Super-poder não encontrado", "NOT_FOUND"));

        return Ok(ApiResult<SuperPowerDto>.SuccessResult(result));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<SuperPowerDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateSuperPower([FromBody] CreateSuperPowerCommand command)
    {
        var result = await mediator.Send(command);

        return CreatedAtAction(
            actionName: nameof(GetSuperPowerById),
            routeValues: new { id = result?.Id },
            value: ApiResult<SuperPowerDto>.SuccessResult(result));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResult<SuperPowerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<SuperPowerDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSuperPower(int id, [FromBody] UpdateSuperPowerCommand command)
    {
        command.SetHeroId(id);
        var result = await mediator.Send(command);

        if (result is null)
            return NotFound(ApiResult<SuperPowerDto>.FailureResult("Herói não encontrado", "NOT_FOUND"));

        return Ok(ApiResult<SuperPowerDto>.SuccessResult(result));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResult<SuperPowerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<SuperPowerDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteHero(int id)
    {
        var result = await mediator.Send(new DeleteSuperPowerCommand(id));

        if (result is null)
            return NotFound(ApiResult<SuperPowerDto>.FailureResult("Herói não encontrado", "NOT_FOUND"));

        return Ok(ApiResult<SuperPowerDto>.SuccessResult(result));
    }
}