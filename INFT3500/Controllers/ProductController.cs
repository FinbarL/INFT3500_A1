using INFT3500.Models;
using INFT3500.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
        return View(productViewModel);
    }

    private async Task<List<ProductViewModel>> GetProductList(string? searchString)
    {
        var products = _dbContext.Products
            .Include(p => p.GenreNavigation)
            .Include(p => p.Stocktakes).ThenInclude(s => s.Source)
            .Where(p => p.Author != null && p.Name != null &&
                        (searchString == null || searchString == "" ||
                         p.Name.Contains(searchString) || p.Author.Contains(searchString)) && p.Stocktakes.Count(s => s.Source.ExternalLink != null) > 0)
            .Select(p => ProductToViewModel(p));
        return await products.ToListAsync();
    }

    [Authorize(Policy = "RequireAdminRole")]
    public IActionResult AddItem()
    {
        var model = new AddProductViewModel();
        model.Genre = 1;
        return View(model);
    }
    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost]
    public IActionResult AddItem(Product product)
    {
        return View();
    }
    private static ProductViewModel ProductToViewModel(Product product)
    {

        if (product == null)
        {
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
            SubGenre = product.SubGenre,
            Quantity = product.Stocktakes.Where(s => s.Source.ExternalLink != null).Select(s => s.Quantity).Sum() ?? 0, 
            Price = product.Stocktakes.Where(s => s.Source.ExternalLink != null).Select(s => s.Price).FirstOrDefault() ?? 0,
        };
        return productViewModel;
    }
    private async Task<ProductViewModel> GetProductById(int id)
    {
        var productViewModel = _dbContext.Products
            .Include(p => p.GenreNavigation)
            .Include(p => p.Stocktakes).ThenInclude(s => s.Source)
            .Where(p => p.Id == id)
            .Select(p => ProductToViewModel(p)).First();
        return productViewModel;
    }

}