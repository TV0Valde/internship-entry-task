using TicTacToeAPI.Models;

namespace TicTacToeAPI.Repositories.Interfaces;

/// <summary>
/// Репозиторий для работы с <see cref="GameEntity"/>
/// </summary>
public interface IGameRepository
{
    /// <summary>
    /// Найти игру в файловой системе/бд
    /// </summary>
    /// <param name="id">Идентификатор игры</param>
    /// <returns>Результат выполения задачи</returns>
    Task<GameEntity?> GetAsync(Guid id);

    /// <summary>
    /// Получить все записи из файловой системы/бд
    /// </summary>
    /// <returns>Коллекция сущностей</returns>
    Task<IEnumerable<GameEntity?>> GetAllAsync();

    /// <summary>
    /// Сохранение записи в файловой системе/бд
    /// </summary>
    /// <param name="game">Сущность игры</param>
    /// <returns>Результат выполения задачи</returns>
    Task SaveAsync(GameEntity game);

    /// <summary>
    /// Удаление игры
    /// </summary>
    /// <param name="id">Идентификатор игры</param>
    /// <returns>Результат выполения задачи</returns>
    Task DeleteAsync(Guid id);
}
