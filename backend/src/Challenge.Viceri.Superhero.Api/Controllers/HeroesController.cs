using System.Net.Mime;
using Challenge.Viceri.Superhero.Api.Commons;
using Challenge.Viceri.Superhero.Application.UseCases.Heros.Commands;
using Challenge.Viceri.Superhero.Application.UseCases.Heros.DataTransferObjects;
using Challenge.Viceri.Superhero.Application.UseCases.Heros.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Challenge.Viceri.Superhero.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class HeroesController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<HeroDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<HeroDto>>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllHeroes()
    {
        var result = await mediator.Send(new GetAllHeroesQuery());

        if (!result.Any())
            return NotFound(ApiResult<IEnumerable<HeroDto>>.FailureResult("Nenhum herói cadastrado", "NOT_FOUND"));

        return Ok(ApiResult<IEnumerable<HeroDto>>.SuccessResult(result));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResult<HeroDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<HeroDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHeroById(int id)
    {
        var result = await mediator.Send(new GetHeroByIdQuery(id));

        if (result is null)
            return NotFound(ApiResult<HeroDto>.FailureResult("Herói não encontrado", "NOT_FOUND"));

        return Ok(ApiResult<HeroDto>.SuccessResult(result));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<HeroDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateHero([FromBody] CreateHeroCommand command)
    {
        var result = await mediator.Send(command);

        return CreatedAtAction(
            actionName: nameof(GetHeroById),
            routeValues: new { id = result?.Id },
            value: ApiResult<HeroDto>.SuccessResult(result));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResult<HeroDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<HeroDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateHero(int id, [FromBody] UpdateHeroCommand command)
    {
        command.SetHeroId(id);
        var result = await mediator.Send(command);

        if (result is null)
            return NotFound(ApiResult<HeroDto>.FailureResult("Herói não encontrado", "NOT_FOUND"));

        return Ok(ApiResult<HeroDto>.SuccessResult(result));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResult<HeroDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<HeroDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteHero(int id)
    {
        var result = await mediator.Send(new DeleteHeroCommand(id));

        if (result is null)
            return NotFound(ApiResult<HeroDto>.FailureResult("Herói não encontrado", "NOT_FOUND"));

        return Ok(ApiResult<HeroDto>.SuccessResult(result));
    }
}