using System.ComponentModel.DataAnnotations;

namespace GameStore.Shared.Models
{
    public class Game
    {
        public int GameId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int? GenreId { get; set; }
        public Genre? Genre { get; set; }
        [Range(1, 100)]
        public double Price { get; set; }
        public DateOnly ReleaseDate { get; set; }
    }
}
