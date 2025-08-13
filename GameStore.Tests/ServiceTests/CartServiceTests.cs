//using GameStore.Data;
//using GameStore.Services;
//using GameStore.Shared.Models;
//using Microsoft.EntityFrameworkCore;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace GameStore.Tests.ServiceTests
//{
//    public class CartServiceTests
//    {
//        private readonly CartService _cartService;

//        public CartServiceTests()
//        {
//            var options = new DbContextOptionsBuilder<GameStoreContext>()
//                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
//                .Options;

//            var mockContext = new Mock<GameStoreContext>(options);
//            _cartService = new CartService(mockContext.Object);
//        }

//        [Fact]
//        public async Task GetCartGamesAsync_NullCartItems_ReturnsEmptyList()
//        {
//            var result = await _cartService.GetCartGamesAsync(null);

//            Assert.True(result.Success);
//            Assert.Empty(result.Data!);
//        }

//        [Fact]
//        public async Task GetCartGamesAsync_EmptyCartItems_ReturnsEmptyList()
//        {
//            var result = await _cartService.GetCartGamesAsync(new List<CartItem>());

//            Assert.True(result.Success);
//            Assert.Empty(result.Data!);
//        }

//        [Fact]
//        public async Task GetCartGamesAsync_ValidCartItems_ReturnsCorrectGames()
//        {
//            var options = new DbContextOptionsBuilder<GameStoreContext>()
//                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
//                .Options;

//            using var context = new GameStoreContext(options);

//            context.Genres.Add(new Genre { GenreId = 1, Name = "Action" });

//            context.Games.Add(new Game
//            {
//                GameId = 1,
//                Name = "Test Game",
//                Price = 10,
//                GenreId = 1,
//                ImageUrl = "test.jpg"
//            });

//            await context.SaveChangesAsync();

//            var service = new CartService(context);
//            var cartItems = new List<CartItem>
//            {
//                new CartItem { GameId = 1, Quantity = 2 }
//            };

//            var result = await service.GetCartGamesAsync(cartItems);

//            Assert.True(result.Success);
//            if (result.Data != null)
//            {
//                Assert.Single(result.Data);
//                Assert.Equal("Test Game", result.Data[0]!.Name);
//                Assert.Equal(2, result.Data[0]!.Quantity);
//            }
//        }



//    }
//}