using System.Text.Json;
using TicTacToeAPI.Models;
using TicTacToeAPI.Repositories.Interfaces;

namespace TicTacToeAPI.Repositories.FileGameRepository;

/// <summary>
/// Реализация <inheritdoc cref="IGameRepository" path="/summary/node()"/> через файловую систему
public class FileGameRepository : IGameRepository
{

    /// <summary>
    /// Название папки хранения игровых записей
    /// </summary>
    private readonly string _directory = "GamesData";

    /// <summary>
    /// Инициализация нового экземпляра <see cref="FileGameRepository"/>
    /// </summary>
    public FileGameRepository()
    {
        if (!Directory.Exists(_directory))
            Directory.CreateDirectory(_directory);
    }

    /// <inheritdoc/>
    public Task DeleteAsync(Guid id)
    {
        var path = Path.Combine(_directory, $"{id}.json");

        if (File.Exists(path))
            File.Delete(path);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<GameEntity?>> GetAllAsync()
    {
        var files = Directory.GetFiles(_directory, "*.json");
        var result = new List<GameEntity>();

        foreach (var file in files)
        {
            var json = await File.ReadAllTextAsync(file);
            var game = JsonSerializer.Deserialize<GameEntity>(json);
            if (game is not null)
                result.Add(game);
        }

        return result;
    }

    /// <inheritdoc/>
    public Task<GameEntity?> GetAsync(Guid id)
    {
        var path = Path.Combine(_directory, $"{id}.json");
        if (!File.Exists(path))
            return Task.FromResult<GameEntity?>(null);

        var json = File.ReadAllText(path);
        var game = JsonSerializer.Deserialize<GameEntity>(json);
        return Task.FromResult(game);
    }

    /// <inheritdoc/>
    public async Task SaveAsync(GameEntity game)
    {
        var path = Path.Combine(_directory, $"{game.Id}.json");
        var json = JsonSerializer.Serialize(game, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(path, json);

    }
}
