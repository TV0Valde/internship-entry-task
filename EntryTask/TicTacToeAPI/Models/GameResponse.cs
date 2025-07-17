namespace TicTacToeAPI.Models;

/// <summary>
/// Модель для возврата текущего состояния игры в API
/// </summary>
public class GameResponse
{
    public Guid GameId { get; set; }
    public string[] Board { get; set; } = default!;
    public string CurrentPlayer { get; set; } = default!;
    public string Status { get; set; } = default!;
}

