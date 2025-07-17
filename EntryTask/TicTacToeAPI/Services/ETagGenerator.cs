using System.Security.Cryptography;
using System.Text;
using TicTacToeAPI.Models;

namespace TicTacToeAPI.Services;

public class ETagGenerator
{
    public static string Generate(GameResponse game)
    {
        var raw = game.GameId + string.Join("", game.Board.Cast<string>());
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
        return Convert.ToBase64String(hash);
    }
}
