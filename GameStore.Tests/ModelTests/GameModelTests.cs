using GameStore.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GameStore.Tests.ModelTests
{
    public class GameModelTests
    {
        private static List<ValidationResult> ValidateModel(Game game)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(game, null, null);
            Validator.TryValidateObject(game, context, validationResults, true);
            return validationResults;
        }

        [Fact]
        public void Game_WithValidData_ShouldBeValid()
        {
            // Arrange
            var game = new Game
            {
                Name = "Test Game",
                GenreId = 1,
                Price = 50,
                ReleaseDate = DateOnly.FromDateTime(DateTime.Today),
                ImageData = new byte[] { 1, 2, 3 }
            };

            // Act
            var results = ValidateModel(game);

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void Game_WithoutName_ShouldBeInvalid()
        {
            var game = new Game
            {
                GenreId = 1,
                Price = 50,
                ReleaseDate = DateOnly.FromDateTime(DateTime.Today),
                ImageData = new byte[] { 1, 2, 3 }
            };

            var results = ValidateModel(game);

            Assert.Contains(results, r => r.MemberNames.Contains("Name"));
        }

        [Fact]
        public void Game_WithPriceOutOfRange_ShouldBeInvalid()
        {
            var game = new Game
            {
                Name = "Invalid Game",
                GenreId = 1,
                Price = 150,
                ReleaseDate = DateOnly.FromDateTime(DateTime.Today),
                ImageData = new byte[] { 1, 2, 3 }
            };

            var results = ValidateModel(game);

            Assert.Contains(results, r => r.MemberNames.Contains("Price"));
        }

        [Fact]
        public void Game_WithoutGenreId_ShouldBeInvalid()
        {
            var game = new Game
            {
                Name = "Another Game",
                Price = 30,
                ReleaseDate = DateOnly.FromDateTime(DateTime.Today),
                ImageData = new byte[] { 1, 2, 3 }
            };

            var results = ValidateModel(game);

            Assert.Contains(results, r => r.MemberNames.Contains("GenreId"));
        }

        [Fact]
        public void Game_Should_Have_ReleaseDate_Property_Get()
        {
            var releaseDate = DateOnly.FromDateTime(DateTime.Today);
            var game = new Game
            {
                ReleaseDate = releaseDate
            };

            Assert.Equal(releaseDate, game.ReleaseDate);
        }

        [Fact]
        public void Game_Should_Have_ImageData_Property_Get()
        {
            var imageData = new byte[] { 1, 2, 3, 4, 5 };
            var game = new Game
            {
                ImageData = imageData
            };

            Assert.Equal(imageData, game.ImageData);
            Assert.Equal(5, game.ImageData.Length);
        }

        [Fact]
        public void Game_Should_Have_ImageData_Property_Set()
        {
            var game = new Game();
            var imageData = new byte[] { 10, 20, 30 };

            game.ImageData = imageData;

            Assert.Equal(imageData, game.ImageData);
            Assert.Equal(3, game.ImageData.Length);
        }

        [Fact]
        public void Game_Should_Have_Nullable_ImageData()
        {
            var game = new Game();

            game.ImageData = null;

            Assert.Null(game.ImageData);
        }

        [Fact]
        public void Game_Should_Allow_Future_ReleaseDate()
        {
            var futureDate = DateOnly.FromDateTime(DateTime.Today.AddDays(30));
            var game = new Game
            {
                Name = "Future Game",
                GenreId = 1,
                Price = 60,
                ReleaseDate = futureDate,
                ImageData = new byte[] { 1, 2, 3 }
            };

            var results = ValidateModel(game);

            Assert.Empty(results);
            Assert.Equal(futureDate, game.ReleaseDate);
        }

        [Fact]
        public void Game_Should_Allow_Past_ReleaseDate()
        {
            var pastDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-365));
            var game = new Game
            {
                Name = "Old Game",
                GenreId = 1,
                Price = 20,
                ReleaseDate = pastDate,
                ImageData = new byte[] { 1, 2, 3 }
            };

            var results = ValidateModel(game);

            Assert.Empty(results);
            Assert.Equal(pastDate, game.ReleaseDate);
        }

        [Fact]
        public void Game_Should_Have_GenreId_Property_GetSet()
        {
            var game = new Game();

            game.GenreId = 5;

            Assert.Equal(5, game.GenreId);
        }

        [Fact]
        public void Game_Should_Have_Name_Property_GetSet()
        {
            var game = new Game();

            game.Name = "Test Game Name";

            Assert.Equal("Test Game Name", game.Name);
        }
    }
}