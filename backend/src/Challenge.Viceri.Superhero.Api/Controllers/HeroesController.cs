using System.Net.Mime;
using Challenge.Viceri.Superhero.Api.Commons;
using Challenge.Viceri.Superhero.Application.UseCases.Heros.Commands;
using Challenge.Viceri.Superhero.Application.UseCases.Heros.DataTransferObjects;
using Challenge.Viceri.Superhero.Application.UseCases.Heros.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Challenge.Viceri.Superhero.Api.Controllers;

/// <summary>
/// Endpoint dos hérois
/// </summary>
/// <param name="mediator">Injeçao de dependência do mediator</param>
[ApiController]
[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class HeroesController(ISender mediator) : ControllerBase
{
    /// <summary>
    /// Busca a lista de hérois cadastrados
    /// </summary>
    /// <returns>Lista de hérois cadastrados</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<HeroDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<HeroDto>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<HeroDto>>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<HeroDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllHeroes()
    {
        var result = await mediator.Send(new GetAllHeroesQuery());

        if (!result.Any())
            return NotFound(ApiResult<IEnumerable<HeroDto>>.FailureResult("Nenhum herói cadastrado", "NOT_FOUND"));

        return Ok(ApiResult<IEnumerable<HeroDto>>.SuccessResult(result));
    }

    /// <summary>
    /// Busca um héroi por id
    /// </summary>
    /// <param name="id">Id do héroi a ser localizado</param>
    /// <returns>Héroi procurado pelo id</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResult<HeroDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<HeroDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult<HeroDto>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResult<HeroDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetHeroById(int id)
    {
        var result = await mediator.Send(new GetHeroByIdQuery(id));

        if (result is null)
            return NotFound(ApiResult<HeroDto>.FailureResult("Herói não encontrado", "NOT_FOUND"));

        return Ok(ApiResult<HeroDto>.SuccessResult(result));
    }

    /// <summary>
    /// Cadastrar um héroi
    /// </summary>
    /// <param name="command">Objeto contendo as informações para cadastro de um héroi</param>
    /// <returns>Héroi cadastrado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<HeroDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResult<HeroDto>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResult<HeroDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateHero([FromBody] CreateHeroCommand command)
    {
        var result = await mediator.Send(command);

        return CreatedAtAction(
            actionName: nameof(GetHeroById),
            routeValues: new { id = result?.Id },
            value: ApiResult<HeroDto>.SuccessResult(result));
    }

    /// <summary>
    /// Atualiza informações de um héroi
    /// </summary>
    /// <param name="id">Id do héroi a ser atualizado</param>
    /// <param name="command">Objeto contendo as informações de atualização de um héroi</param>
    /// <returns>Héroi atualizado</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResult<HeroDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<HeroDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult<HeroDto>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResult<HeroDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateHero(int id, [FromBody] UpdateHeroCommand command)
    {
        command.SetHeroId(id);
        var result = await mediator.Send(command);

        if (result is null)
            return NotFound(ApiResult<HeroDto>.FailureResult("Herói não encontrado", "NOT_FOUND"));

        return Ok(ApiResult<HeroDto>.SuccessResult(result));
    }

    /// <summary>
    /// Exclui um héroi
    /// </summary>
    /// <param name="id">Id do héroi a ser excluído</param>
    /// <returns>Héroi excluído</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResult<HeroDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<HeroDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult<HeroDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteHero(int id)
    {
        var result = await mediator.Send(new DeleteHeroCommand(id));

        if (result is null)
            return NotFound(ApiResult<HeroDto>.FailureResult("Herói não encontrado", "NOT_FOUND"));

        return Ok(ApiResult<HeroDto>.SuccessResult(result));
    }
}