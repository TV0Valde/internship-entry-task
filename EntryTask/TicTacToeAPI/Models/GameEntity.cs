namespace TicTacToeAPI.Models
{
    public class GameEntity
    {
        public Guid Id { get; set; }
        public int BoardSize { get; set; }
        public string SerializedBoard { get; set; } = string.Empty;
        public string CurrentPlayer { get; set; } = "X";
        public string Status { get; set; } = "Active";
        public int MovesCount { get; set; }
    }

}
