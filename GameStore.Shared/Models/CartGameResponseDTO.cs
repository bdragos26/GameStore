namespace GameStore.Shared.Models
{
    public class CartGameResponseDTO
    {
        public int GameId { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}
