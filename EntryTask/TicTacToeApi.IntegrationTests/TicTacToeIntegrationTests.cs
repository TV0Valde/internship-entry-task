using System.Net;
using System.Net.Http.Json;
using TicTacToeAPI.Models;
using Xunit;

namespace TicTacToeApi.IntegrationTests;

public class TicTacToeIntegrationTests : IClassFixture<TicTacToeApiFactory>
{

    private readonly HttpClient _client;

    public TicTacToeIntegrationTests(TicTacToeApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateAndGetGame_ShouldReturnGame()
    {
        // Act
        var createResponse = await _client.PostAsync("api/games", null);
        createResponse.EnsureSuccessStatusCode();

        var game = await createResponse.Content.ReadFromJsonAsync<GameResponse>();

        // Act 
        var getResponse = await _client.GetAsync($"api/games/{game?.GameId}");
        getResponse.EnsureSuccessStatusCode();

        var retrievedGame = await getResponse.Content.ReadFromJsonAsync<GameResponse>();

        // Assert
        Assert.Equal(game?.GameId, retrievedGame?.GameId);
        Assert.Equal(game?.CurrentPlayer, retrievedGame?.CurrentPlayer);
        Assert.Equal(game?.Status, retrievedGame?.Status);
    }

    [Fact]
    public async Task Get_NonExistentGame_ShouldReturnNotFound()
    {
        var response = await _client.GetAsync($"api/games/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task MakeMove_ShouldUpdateBoard()
    {
        var createResponse = await _client.PostAsync("api/games", null);
        createResponse.EnsureSuccessStatusCode();

        var game = await createResponse.Content.ReadFromJsonAsync<GameResponse>();

        var move = new { Row = 0, Column = 0, Player = "X" };
        var moveResponse = await _client.PostAsJsonAsync($"api/games/{game?.GameId}/move", move);
        moveResponse.EnsureSuccessStatusCode();

        var updateGame = await moveResponse.Content.ReadFromJsonAsync<GameResponse>();
        Assert.Equal("O", updateGame?.CurrentPlayer);
        Assert.Equal("X", updateGame?.Board[0]);

    }

    [Fact]
    public async Task MakeMove_OnTakenCell_ShouldReturnBadRequest()
    {
        var createResponse = await _client.PostAsync("api/games", null);
        createResponse.EnsureSuccessStatusCode();

        var game = await createResponse.Content.ReadFromJsonAsync<GameResponse>();

        var move = new { Row = 0, Col = 0, Player = "X" };
        await _client.PostAsJsonAsync($"api/games/{game?.GameId}/move", move);

        var badMoveRequesrt = await _client.PostAsJsonAsync($"api/games/{game?.GameId}/move", move);
        Assert.Equal(HttpStatusCode.BadRequest, badMoveRequesrt.StatusCode);
    }

    [Fact]
    public async Task MakeMoveAsync_WhenPlayerMovesOutOfTurn_ShouldReturnBadRequest()
    {
        var createResponse = await _client.PostAsync("api/games", null);
        createResponse.EnsureSuccessStatusCode();

        var game = await createResponse.Content.ReadFromJsonAsync<GameResponse>();

        await _client.PostAsJsonAsync($"api/games/{game?.GameId}/move", new { Row = 0, Column = 0, Player = "X" });

        var badMoveRequest = await _client.PostAsJsonAsync($"api/games/{game?.GameId}/move", new { Row = 1, Column = 0, Player = "X" });
        Assert.Equal(HttpStatusCode.BadRequest, badMoveRequest.StatusCode);

    }

    [Fact]
    public async Task WinCondition_ShouldFinishGame()
    {
        var createResponse = await _client.PostAsync("api/games", null);
        createResponse.EnsureSuccessStatusCode();

        var game = await createResponse.Content.ReadFromJsonAsync<GameResponse>();

        await _client.PostAsJsonAsync($"api/games/{game?.GameId}/move", new { Row = 0, Column = 0, Player = "X" });
        await _client.PostAsJsonAsync($"api/games/{game?.GameId}/move", new { Row = 2, Column = 2, Player = "O" });
        await _client.PostAsJsonAsync($"api/games/{game?.GameId}/move", new { Row = 0, Column = 1, Player = "X" });
        await _client.PostAsJsonAsync($"api/games/{game?.GameId}/move", new { Row = 1, Column = 1, Player = "O" });
        await _client.PostAsJsonAsync($"api/games/{game?.GameId}/move", new { Row = 0, Column = 2, Player = "X" });

        var getResponse = await _client.GetAsync($"api/games/{game?.GameId}");
        var updatedGame = await getResponse.Content.ReadFromJsonAsync<GameResponse>();

        Assert.Equal("XWin", updatedGame?.Status);
    }

    [Fact]
    public async Task DrawCondition_ShouldFinishGame()
    {
        var createResponse = await _client.PostAsync("api/games", null);
        createResponse.EnsureSuccessStatusCode();

        var game = await createResponse.Content.ReadFromJsonAsync<GameResponse>();

        await _client.PostAsJsonAsync($"api/games/{game?.GameId}/move", new { Row = 0, Column = 0, Player = "X" });
        await _client.PostAsJsonAsync($"api/games/{game?.GameId}/move", new { Row = 0, Column = 1, Player = "O" });
        await _client.PostAsJsonAsync($"api/games/{game?.GameId}/move", new { Row = 0, Column = 2, Player = "X" });
        await _client.PostAsJsonAsync($"api/games/{game?.GameId}/move", new { Row = 1, Column = 1, Player = "O" });
        await _client.PostAsJsonAsync($"api/games/{game?.GameId}/move", new { Row = 1, Column = 0, Player = "X" });
        await _client.PostAsJsonAsync($"api/games/{game?.GameId}/move", new { Row = 2, Column = 0, Player = "O" });
        await _client.PostAsJsonAsync($"api/games/{game?.GameId}/move", new { Row = 2, Column = 1, Player = "X" });
        await _client.PostAsJsonAsync($"api/games/{game?.GameId}/move", new { Row = 1, Column = 2, Player = "O" });
        await _client.PostAsJsonAsync($"api/games/{game?.GameId}/move", new { Row = 2, Column = 2, Player = "X" });

        var getResponse = await _client.GetAsync($"api/games/{game?.GameId}");
        var updateGame = await getResponse.Content.ReadFromJsonAsync<GameResponse>();

        Assert.Equal("Draw", updateGame?.Status);
    }
}
