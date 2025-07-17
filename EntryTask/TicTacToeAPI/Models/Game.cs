namespace TicTacToeAPI.Models;

/// <summary>
/// Модель игры.
/// </summary>
public class Game
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int BoardSize { get; set; }
    public string[,] Board { get; set; }
    public string Status { get; set; } = "Active";
    public string CurrentPlayer { get; set; } = "X";
    public int MovesCount { get; set; } = 0;
}
