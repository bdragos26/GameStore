using GameStore.Shared.DTOs;
using GameStore.Shared.Models;

namespace GameStore.Shared.Mappers
{
    public static class GameRatingMapper
    {
        public static GameRatingDTO ToDto(IGrouping<Game?, GameRating> group)
        {
            return new GameRatingDTO
            {
                Game = group.Key,
                AverageScore = group.Average(r => r.Score),
                RatingCount = group.Count()
            };
        }
    }
}
