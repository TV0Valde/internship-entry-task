using TicTacToeAPI.Models;

namespace TicTacToeAPI.Services.Interfaces;

/// <summary>
/// Сервис игры.
/// </summary>
public interface IGameService
{
    /// <summary>
    /// Создание новой игры.
    /// </summary>
    /// <returns>Новая игра</returns>
    Task<GameResponse> CreateNewGameAsync();

    /// <summary>
    /// Обработка хода игрока.
    /// </summary>
    /// <param name="gameId">Id игры</param>
    /// <param name="request">Параметры хода</param>
    Task<GameResponse?> MakeMoveAsync(Guid gameId, MoveRequest request);
    
    /// <summary>
    /// Получение игры по Id.
    /// </summary>
    /// <param name="gameId">Id игры</param>
    /// <returns>Существующая игра</returns>
    Task<GameResponse?> GetGameAsync(Guid gameId);
}
