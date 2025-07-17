using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Moq;
using TicTacToeAPI.Models;
using TicTacToeAPI.Repositories.Interfaces;
using TicTacToeAPI.Services;

namespace TicTacToeApi.Tests;

public class GameServiceTests
{
    private readonly Mock<IGameRepository> _repositoryMock;
    private readonly GameService _service;

    public GameServiceTests()
    {
        _repositoryMock = new Mock<IGameRepository>();
        Environment.SetEnvironmentVariable("BOARD_SIZE", "3");
        Environment.SetEnvironmentVariable("WIN_CONDITION", "3");

        var config = new ConfigurationBuilder().Build();
        _service = new GameService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateNewGame_ShouldReturn_ValidGame()
    {
        // Act
        var result = await _service.CreateNewGameAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(9, result.Board.Length);
        Assert.Equal("X", result.CurrentPlayer);
        Assert.Equal("Active", result.Status);
        _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<GameEntity>()), Times.Once);
    }

    [Fact]
    public async Task MakeMoveAsync_ShouldMakeMoveSuccessfully()
    {
        // Arrange
        var board = new string[9];
        var entity = new GameEntity
        {
            Id = Guid.NewGuid(),
            BoardSize = 3,
            SerializedBoard = JsonSerializer.Serialize(board.Cast<string?>().Select(x => x ?? "").ToArray()),
            CurrentPlayer = "X",
            Status = "Active",
            MovesCount = 0
        };

        _repositoryMock.Setup(r => r.GetAsync(entity.Id)).ReturnsAsync(entity);

        var request = new MoveRequest { Row = 0, Column = 0, Player = "X" };

        // Act
        var result = await _service.MakeMoveAsync(entity.Id, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("O", result.CurrentPlayer);
        Assert.Equal("Active", result.Status);
        Assert.Equal("X", result.Board[0]);
        _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<GameEntity>()), Times.Once);
    }

    [Fact]
    public async Task MakeMoveAsync_ShouldThrow_WhenCellOccupied()
    {
        var board = new string[9];
        board[0] = "X";
        var entity = new GameEntity
        {
            Id = Guid.NewGuid(),
            SerializedBoard = JsonSerializer.Serialize(board.Cast<string?>().Select(x => x ?? "").ToArray()),
            BoardSize = 3,
            CurrentPlayer = "Y",
            Status = "Active",
            MovesCount = 1,
        };

        _repositoryMock.Setup(x => x.GetAsync(entity.Id)).ReturnsAsync(entity);

        var request = new MoveRequest { Row = 0, Column = 0, Player = "X" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.MakeMoveAsync(entity.Id, request));
    }


    [Fact]
    public async Task MakeMoveAsync_ShouldThrow_WhenPlayerMovesOutOfTurn()
    {
        var board = new string[9];
        board[0] = "X";
        var entity = new GameEntity
        {
            Id = Guid.NewGuid(),
            SerializedBoard = JsonSerializer.Serialize(board.Cast<string?>().Select(x => x ?? "").ToArray()),
            BoardSize = 3,
            CurrentPlayer = "Y",
            Status = "Active",
            MovesCount = 1,
        };

        _repositoryMock.Setup(x => x.GetAsync(entity.Id)).ReturnsAsync(entity);

        var request = new MoveRequest { Row = 1, Column = 0, Player = "X" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.MakeMoveAsync(entity.Id, request));
    }

    [Fact]
    public async Task MakeMoveAsync_ShouldReturnNull_IfGameEnd()
    {
        // Arrange
        var board = new string[9];
        var entity = new GameEntity
        {
            Id = Guid.NewGuid(),
            BoardSize = 3,
            SerializedBoard = JsonSerializer.Serialize(board.Cast<string?>().Select(x => x ?? "").ToArray()),
            CurrentPlayer = "X",
            Status = "Xwin",
            MovesCount = 5
        };

        _repositoryMock.Setup(r => r.GetAsync(entity.Id)).ReturnsAsync(entity);

        var request = new MoveRequest { Row = 0, Column = 0, Player = "X" };

        // Act
        var result = await _service.MakeMoveAsync(entity.Id, request);

        Assert.Null(result);
    }


    [Fact]
    public async Task GetGameAsync_ShouldReturnGame_WhenExists()
    {

        // Arrange
        var board = new string[9];
        var entity = new GameEntity
        {
            Id = Guid.NewGuid(),
            BoardSize = 3,
            SerializedBoard = JsonSerializer.Serialize(board.Cast<string?>().Select(x => x ?? "").ToArray()),
            CurrentPlayer = "X",
            Status = "Active"
        };

        _repositoryMock.Setup(x => x.GetAsync(entity.Id)).ReturnsAsync(entity);

        var result = await _service.GetGameAsync(entity.Id);

        Assert.NotNull(result);
        Assert.Equal(entity.Id, result.GameId);
        Assert.Equal("X", result.CurrentPlayer);
        Assert.Equal("Active", result.Status);
    }

    [Fact]
    public async Task GetGameAsyns_ShoudReturn_Game_WhenDraw()
    {
        // Arrange
        var board = new string[9];
        var entity = new GameEntity
        {
            Id = Guid.NewGuid(),
            BoardSize = 3,
            SerializedBoard = JsonSerializer.Serialize(board.Cast<string?>().Select(x => x ?? "").ToArray()),
            CurrentPlayer = "X",
            Status = "Draw",
            MovesCount = 9
        };

        _repositoryMock.Setup(x => x.GetAsync(entity.Id)).ReturnsAsync(entity);

        var result = await _service.GetGameAsync(entity.Id);

        Assert.NotNull(result);
        Assert.Equal(entity.Id, result.GameId);
        Assert.Equal("X", result.CurrentPlayer);
        Assert.Equal("Draw", result.Status);
    }


}
