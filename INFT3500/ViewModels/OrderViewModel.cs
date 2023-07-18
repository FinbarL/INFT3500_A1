namespace INFT3500.ViewModels;

public class OrderViewModel
{
    public int OrderId { get; set; }
    public string StreetAddress { get; set; }
    public string Suburb { get; set; }
    public string State { get; set; }
    public PatronViewModel Customer { get; set; }
}