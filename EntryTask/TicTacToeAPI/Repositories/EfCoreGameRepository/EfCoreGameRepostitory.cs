using Microsoft.EntityFrameworkCore;
using TicTacToeAPI.Db;
using TicTacToeAPI.Models;
using TicTacToeAPI.Repositories.Interfaces;

namespace TicTacToeAPI.Repositories.EfCoreGameRepository;

/// <summary>
/// Реализация <inheritdoc cref="IGameRepository" path="/summary/node()"/> через бд
/// </summary>
public class EfCoreGameRepostitory : IGameRepository
{
    /// <summary>
    /// <inheritdoc cref="TicTacToeDbContext" path="/summary/node()"/>
    /// </summary>
    private readonly TicTacToeDbContext _context;

    /// <summary>
    /// Инициализация нового экземпляра <see cref="EfCoreGameRepository"/>
    /// </summary>
    /// <param name="context"> <inheritdoc cref="TicTacToeDbContext" path="/summary/node()"/></param>
    public EfCoreGameRepostitory(TicTacToeDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(Guid id)
    {
        var entity = await _context.Games.FindAsync(id);
        if (entity is not null)
        {
            _context.Games.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<GameEntity?>> GetAllAsync()
    {
        return await _context.Games.ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<GameEntity?> GetAsync(Guid id)
    {
        return await _context.Games.FindAsync(id);
    }

    /// <inheritdoc/>
    public async Task SaveAsync(GameEntity game)
    {
        var existing = await _context.Games.FindAsync(game.Id);
        if (existing is null)
        {
            await _context.Games.AddAsync(game);
        }
        else
        {
            _context.Entry(existing).CurrentValues.SetValues(game);
        }

        await _context.SaveChangesAsync();
    }
}
