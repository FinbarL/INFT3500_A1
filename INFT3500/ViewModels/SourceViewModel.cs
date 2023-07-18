namespace INFT3500.ViewModels;

public class SourceViewModel
{
    public int SourceId { get; set; }
    public string SourceName { get; set; }
    public string ExternalLink { get; set; }
    public GenreViewModel Genre { get; set; }
}