using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicTacToeAPI.Models;
using TicTacToeAPI.Services;
using TicTacToeAPI.Services.Interfaces;

namespace TicTacToeAPI.Controllers;

/// <summary>
/// Контроллер игры.
/// </summary>
[Route("api/games")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;

    /// <summary>
    /// Инициализация нового экземпляра <see cref="GameController"/>
    /// </summary>
    public GameController(IGameService gameService)
    {
        _gameService = gameService;
    }

    /// <summary>
    /// Создание новой игры.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<GameResponse>> CreateNewGame()
    {
        var game = await _gameService.CreateNewGameAsync();
        return Ok(game);
    }

    /// <summary>
    /// Получение игры.
    /// </summary>
    /// <param name="gameId"></param>
    [HttpGet("{gameId}")]
    public async Task<ActionResult<GameResponse>> GetGame(Guid gameId)
    {
        var game = await _gameService.GetGameAsync(gameId);
        if (game is null)
        {
            return StatusCode(404, "Игра не найдена");
        }

        return Ok(game);
    }

    /// <summary>
    /// Обработка хода игрока.
    /// </summary>
    [HttpPost("{gameId}/move")]
    public async Task<ActionResult<GameResponse>> MakeMove(Guid gameId, [FromBody] MoveRequest request)
    {
        try
        {
            var existingGame = await _gameService.GetGameAsync(gameId);
            if (existingGame is null)
            {
                return NotFound("Игра не найдена");
            }

            var updatedGame = await _gameService.MakeMoveAsync(gameId, request);
            if (updatedGame is null)
            {
                return NotFound("Игра не найдена");
            }

            var etag = ETagGenerator.Generate(updatedGame);
            Response.Headers["ETag"] = etag;

            return Ok(updatedGame);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

}

