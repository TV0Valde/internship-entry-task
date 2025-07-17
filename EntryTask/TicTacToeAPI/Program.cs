using Microsoft.EntityFrameworkCore;
using TicTacToeAPI.Db;
using TicTacToeAPI.Repositories.EfCoreGameRepository;
using TicTacToeAPI.Repositories.FileGameRepository;
using TicTacToeAPI.Repositories.Interfaces;
using TicTacToeAPI.Services;
using TicTacToeAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IGameService, GameService>();

builder.Services.AddDbContext<TicTacToeDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<FileGameRepository>();
builder.Services.AddScoped<EfCoreGameRepostitory>();
builder.Services.AddScoped<IGameRepository>(provider =>
{
    var storageType = Environment.GetEnvironmentVariable("STORAGE_TYPE");
    return storageType == "DB"
        ? provider.GetRequiredService<EfCoreGameRepostitory>()
        : provider.GetRequiredService<FileGameRepository>();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();


public partial class Program { }
