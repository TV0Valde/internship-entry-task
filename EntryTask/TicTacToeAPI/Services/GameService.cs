using System.Text.Json;
using TicTacToeAPI.Models;
using TicTacToeAPI.Repositories.Interfaces;
using TicTacToeAPI.Services.Interfaces;

namespace TicTacToeAPI.Services;

/// <inheritdoc cref="IGameService"/>
public class GameService : IGameService
{
    /// <summary>
    /// <inheritdoc cref="IGameRepository"/>
    /// </summary>
    private readonly IGameRepository _repository;

    /// <summary>
    /// Размерность игрового поля
    /// </summary>
    private readonly int _boardSize;

    /// <summary>
    /// Длина выигрышной последовательности
    /// </summary>
    private readonly int _winLength;


    private readonly Random _random = new();

    /// <summary>
    /// Инициализация экземпляра <see cref="GameService"/>
    /// </summary>
    /// <param name="repository"> <inheritdoc cref="IGameRepository" path="/summary/node()"/> </param>
    public GameService(IGameRepository repository)
    {
        _repository = repository;
        _boardSize = int.TryParse(Environment.GetEnvironmentVariable("BOARD_SIZE"), out var board) ? board : 3;
        _winLength = int.TryParse(Environment.GetEnvironmentVariable("WIN_CONDITION"), out var win) ? win : 3;
    }

    /// <inheritdoc/>
    public async Task<GameResponse> CreateNewGameAsync()
    {
        var board = new string[_boardSize * _boardSize];

        var entity = new GameEntity
        {
            Id = Guid.NewGuid(),
            BoardSize = _boardSize,
            SerializedBoard = JsonSerializer.Serialize(board),
            CurrentPlayer = "X",
            Status = "Active",
            MovesCount = 0,
        };

        await _repository.SaveAsync(entity);
        return MapToResponse(entity);
    }

    /// <inheritdoc/>
    public async Task<GameResponse?> GetGameAsync(Guid gameId)
    {
        var entity = await _repository.GetAsync(gameId);
        return entity == null ? null : MapToResponse(entity);
    }

    /// <inheritdoc/>
    public async Task<GameResponse?> MakeMoveAsync(Guid gameId, MoveRequest request)
    {
        var entity = await _repository.GetAsync(gameId);

        if (entity is null)
            throw new InvalidOperationException("Игра не найдена");

        if (entity.Status != "Active")
            return null;

        if (entity.CurrentPlayer != request.Player)
            throw new InvalidOperationException($"Сейчас ход {entity.CurrentPlayer}, а не {request.Player}");



        var board = JsonSerializer.Deserialize<string[]>(entity.SerializedBoard)!;
        int index = request.Row * _boardSize + request.Column;

        if (!string.IsNullOrEmpty(board[index]))
            throw new InvalidOperationException("Ячейка занята.");

        var realPlayer = request.Player;
        if (entity.MovesCount > 0 && (entity.MovesCount + 1) % 3 == 0)
        {
            if (_random.Next(1, 101) <= 10)
                realPlayer = request.Player == "X" ? "O" : "X";
        }

        board[index] = realPlayer;
        entity.MovesCount++;
        entity.CurrentPlayer = request.Player == "X" ? "O" : "X";
        entity.SerializedBoard = JsonSerializer.Serialize(board);

        if (CheckWin(board, realPlayer))
        {
            entity.Status = realPlayer == "X" ? "XWin" : "OWin";
        }
        else if (entity.MovesCount >= _boardSize * _boardSize)
        {
            entity.Status = "Draw";
        }

        await _repository.SaveAsync(entity);
        return MapToResponse(entity);
    }

    /// <summary>
    /// Проверка победы
    /// </summary>
    /// <param name="board">Игровре поле</param>
    /// <param name="player">Текущий игроу</param>
    /// <returns></returns>
    private bool CheckWin(string[] board, string player)
    {
        int n = _boardSize;

        bool CheckDirection(int row, int col, int dRow, int dCol)
        {
            int count = 0;
            for (int i = 0; i < _winLength; i++)
            {
                int r = row + dRow * i;
                int c = col + dCol * i;
                int index = r * n + c;
                if (r >= 0 && r < n && c >= 0 && c < n && board[index] == player)
                    count++;
                else
                    break;
            }
            return count == _winLength;
        }

        for (int row = 0; row < n; row++)
        {
            for (int col = 0; col < n; col++)
            {
                int index = row * n + col;
                if (board[index] != player) continue;

                if (CheckDirection(row, col, 0, 1) ||
                    CheckDirection(row, col, 1, 0) ||
                    CheckDirection(row, col, 1, 1) ||
                    CheckDirection(row, col, 1, -1))
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Маппер
    /// </summary>
    /// <param name="entity">Сущность игры</param>
    private GameResponse MapToResponse(GameEntity entity)
    {
        return new GameResponse
        {
            GameId = entity.Id,
            Board = JsonSerializer.Deserialize<string[]>(entity.SerializedBoard)!,
            CurrentPlayer = entity.CurrentPlayer,
            Status = entity.Status,
        };
    }
}
