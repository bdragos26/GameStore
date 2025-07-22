using System.ComponentModel.DataAnnotations;

namespace GameStore.Shared.Models
{
    public class Genre
    {
        public int GenreId { get; set; }
        [Required]
        public required string Name { get; set; }
    }
}
