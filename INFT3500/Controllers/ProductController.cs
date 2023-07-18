using INFT3500.Models;
using INFT3500.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace INFT3500.Controllers;
using Microsoft.AspNetCore.Mvc;
public class ProductController : Controller
{
    private readonly StoreDbContext _dbContext;

    public ProductController(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public IActionResult Index()
    {
        var products = _dbContext.Products.Include(p => p.GenreNavigation).ToList(); // Retrieve products with associated genre

        var productViewModels = products.Select(p => new ProductViewModel
        {
            ProductId = p.Id,
            Name = p.Name,
            Author = p.Author,
            Description = p.Description,
            Published = p.Published,
            Genre = new GenreViewModel
            {
                GenreId = p.GenreNavigation?.GenreId ?? 0, // Access GenreId property, handling null values
                Name = p.GenreNavigation?.Name ?? string.Empty // Access Name property, handling null values
            }
        }).ToList();

        return View(productViewModels);
    }
}