using GameStore.Shared.Models;

namespace GameStore.Shared.DTOs
{
    public class GameRatingDTO
    {
        public Game? Game { get; set; }
        public double AverageScore { get; set; }
        public int RatingCount { get; set; }
    }
}
