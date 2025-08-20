using GameStore.Shared.Models;

namespace GameStore.Tests.ModelTests
{
    public class GameRatingTests
    {
        [Fact]
        public void GameRating_Should_Have_UserId_Property()
        {
            var gameRating = new GameRating();

            gameRating.UserId = 123;

            Assert.Equal(123, gameRating.UserId);
        }

        [Fact]
        public void GameRating_Should_Have_GameId_Property()
        {
            var gameRating = new GameRating();

            gameRating.GameId = 456;

            Assert.Equal(456, gameRating.GameId);
        }

        [Fact]
        public void GameRating_Should_Have_Score_Property()
        {
            var gameRating = new GameRating();

            gameRating.Score = 4;

            Assert.Equal(4, gameRating.Score);
        }

        [Fact]
        public void GameRating_Should_Have_User_Navigation_Property()
        {
            var gameRating = new GameRating();
            var user = new User();

            gameRating.User = user;

            Assert.Equal(user, gameRating.User);
        }

        [Fact]
        public void GameRating_Should_Have_GameDetails_Navigation_Property()
        {
            var gameRating = new GameRating();
            var game = new Game();

            gameRating.GameDetails = game;

            Assert.Equal(game, gameRating.GameDetails);
        }
    }
}
