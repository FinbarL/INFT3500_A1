using System.ComponentModel;
using INFT3500.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace INFT3500.ViewModels;

public class AddProductViewModel
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Author { get; set; }

    public string? Description { get; set; }

    public int? Genre { get; set; }

    public string? SubGenre { get; set; }

    public DateTime? Published { get; set; }

    public int? StocktakeSourceId { get; set; }

    public int? StocktakeQuantity { get; set; }

    public double? StocktakePrice { get; set; }

    [DefaultValue(0)]
    public int? RealQuantity { get; set; }
    public List<SelectListItem> GenreList { get; } = new List<SelectListItem>
    {
        new SelectListItem { Value = "1", Text = "Books" },
        new SelectListItem { Value = "2", Text = "Movies" },
        new SelectListItem { Value = "3", Text = "Games"  },
    };
    public List<SelectListItem> SourceList { get; } = new List<SelectListItem>
    {
        new SelectListItem { Value = "2", Text = "Audible" },
        new SelectListItem { Value = "3", Text = "Steam" },
        new SelectListItem { Value = "4", Text = "Prime Video"  },
    };

}
