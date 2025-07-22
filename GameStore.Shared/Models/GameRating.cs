using System.ComponentModel.DataAnnotations;

namespace GameStore.Shared.Models
{
    public class GameRating
    {
        public int UserId { get; set; }
        public int GameId { get; set; }
        [Range(1, 5)]
        public int Score { get; set; }

        public User? User { get; set; }
        public Game? GameDetails { get; set; }
    }
}
