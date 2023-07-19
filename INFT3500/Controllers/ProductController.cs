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

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var productViewModels = await GetProductList(null);
        return View(productViewModels);
    }

    [HttpPost]
    public async Task<IActionResult> Index(string searchString)
    {
        var productViewModels = await GetProductList(searchString);
        return View(productViewModels);
    }
    public async Task<IActionResult> Details(int id)
    {
        var productViewModel = await GetProductById(id);
        Console.WriteLine(productViewModel);
        return View(productViewModel);
    }

    private async Task<List<ProductViewModel>> GetProductList(string? searchString)
    {
        var products = _dbContext.Products
            .Include(p => p.GenreNavigation)
            .Where(p => p.Author != null && p.Name != null &&
                        (searchString == null || searchString == "" ||
                         p.Name.Contains(searchString) || p.Author.Contains(searchString)))
            .Include(p => p.Stocktakes)
            .Select(p => ProductToViewModel(p));
        return await products.ToListAsync();
    }

    private static ProductViewModel ProductToViewModel(Product product)
    {

        if (product == null)
        {
            Console.WriteLine("ERROR, NO PRODUCT!");
            throw new ArgumentNullException(nameof(product));
        }
        var productViewModel = new ProductViewModel
        {
            ProductId = product.Id,
            Name = product.Name,
            Author = product.Author,
            Description = product.Description,
            Published = product.Published,
            Genre = new GenreViewModel
            {
                GenreId = product.GenreNavigation != null ? product.GenreNavigation.GenreId : 0,
                Name = product.GenreNavigation.Name != null ? product.GenreNavigation.Name : String.Empty,
            },
            Quantity = product.Stocktakes.Select(s => s.Quantity).Sum() ?? 0
        };
        Console.WriteLine("MODEL=" + productViewModel.Author);
        return productViewModel;
    }
    private async Task<ProductViewModel> GetProductById(int id)
    {
        var productViewModel = _dbContext.Products
            .Include(p => p.GenreNavigation)
            .Where(p => p.Id == id)
            .Include(p => p.Stocktakes)
            .Select(p => ProductToViewModel(p)).First();
        Console.WriteLine("WE GOOD!");
            return productViewModel;
    }

}