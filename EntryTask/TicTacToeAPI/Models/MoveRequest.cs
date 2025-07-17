namespace TicTacToeAPI.Models;

/// <summary>
/// Модель входящего запроса на совершение хода.
/// </summary>
public class MoveRequest
{
    public int Row { get; set; }
    public int Column { get; set; }
    public string Player { get; set; }

}
