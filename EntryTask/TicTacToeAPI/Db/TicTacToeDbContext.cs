using Microsoft.EntityFrameworkCore;
using TicTacToeAPI.Models;

namespace TicTacToeAPI.Db;

/// <summary>
/// Контекст базы данных для игры
/// </summary>
public class TicTacToeDbContext : DbContext
{
    public DbSet<GameEntity> Games { get; set; }

    public TicTacToeDbContext(DbContextOptions<TicTacToeDbContext> options)
        : base(options) { }
}
