using INFT3500.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace INFT3500.ViewModels;

public class AddProductViewModel
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Author { get; set; }

    public string? Description { get; set; }

    public int? Genre { get; set; }

    public string? SubGenre { get; set; }

    public DateOnly? Published { get; set; }

    public List<SelectListItem> GenreList { get; } = new List<SelectListItem>
    {
        new SelectListItem { Value = "1", Text = "Books" },
        new SelectListItem { Value = "2", Text = "Movies" },
        new SelectListItem { Value = "3", Text = "Games"  },
    };
}