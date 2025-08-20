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
    }
}
