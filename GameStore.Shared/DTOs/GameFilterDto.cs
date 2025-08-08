namespace GameStore.Shared.DTOs
{
    public class GameFilterDto
    {
        public string? SearchTerm { get; set; }
        public int? GenreId { get; set; }
        public double? MaxPrice { get; set; }
        public DateOnly? MinReleaseDate { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}
