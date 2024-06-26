namespace INFT3500.ViewModels;

public class ProductViewModel
{
    public int ProductId { get; set; }
    public string? Name { get; set; }
    public string? Author { get; set; }
    public string? Description { get; set; }
    public DateTime? Published { get; set; }
    public GenreViewModel Genre { get; set; }
    public string SubGenre { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }
}