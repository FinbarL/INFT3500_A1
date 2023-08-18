using INFT3500.Models;
using INFT3500.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Infrastructure;
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
        var productViewModel = await GetProductViewModelById(id);
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
        Console.WriteLine("Called");
        var model = new AddProductViewModel
        {
            Genre = 1
        };
        return View(model);
    }
    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost]
    public IActionResult AddItem(AddProductViewModel model)
    {
        Console.WriteLine(model.Published);
        Console.WriteLine(new DateTime());
        if (ModelState.IsValid)
        {
            var newProduct = new Product
            {
                Name = model.Name,
                Author = model.Author,
                Description = model.Description,
                Published = model.Published,
                Genre = model.Genre,
                SubGenre = model.SubGenre,
                LastUpdatedBy = User.Identity.Name,
                LastUpdated = DateTime.Now,
            };
            var productAdded = _dbContext.Products.Add(newProduct);
            var newStocktake = new Stocktake
            {
                Product = productAdded.Entity,
                Source = _dbContext.Sources.First(s => s.ExternalLink != null && s.Genre == model.Genre),
                Quantity = model.StocktakeQuantity,
                Price = model.StocktakePrice,
            };
            _dbContext.Stocktakes.Add(newStocktake);
            _dbContext.SaveChanges();
            return RedirectToAction("Details", "Product", new {id = newProduct.Id});
        }
        return View(model);
    }
    public IActionResult EditItem(int id)
    {
        var productViewModel = GetProductById(id);
        var addProductViewModel = new AddProductViewModel
        {
            Name = productViewModel.Name,
            Author = productViewModel.Author,
            Description = productViewModel.Description,
            Published = productViewModel.Published,
            Genre = productViewModel.Genre,
            SubGenre = productViewModel.SubGenre,
            Id = productViewModel.Id,
            StocktakeSourceId = productViewModel.Stocktakes.First(s => s.Source.ExternalLink != null).SourceId,
            StocktakeQuantity = productViewModel.Stocktakes.First(s => s.Source.ExternalLink != null).Quantity,
            StocktakePrice = productViewModel.Stocktakes.First(s => s.Source.ExternalLink != null).Price,
            
        };
        return View(addProductViewModel);
    }
    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost]
    public async Task<IActionResult> EditItem(AddProductViewModel model)
    {
        Console.WriteLine(model.Published);
        Console.WriteLine(new DateTime());
        if (ModelState.IsValid)
        {
            Console.WriteLine("MODELID=" + model.Id);
            var existingItem =  _dbContext.Products.FirstOrDefault(p => p.Id == model.Id);
            var existingStocktake = _dbContext.Stocktakes.FirstOrDefault(s => s.ProductId == model.Id && s.Source.ExternalLink != null);
            Console.WriteLine("NEWSOURCE=" + model.StocktakeSourceId);
            Console.WriteLine("SOURCEID =" + existingStocktake.SourceId);
            if (existingItem != null)
            {
                existingItem.Name = model.Name;
                existingItem.Author = model.Author;
                existingItem.Description = model.Description;
                existingItem.Published = model.Published;
                //existingItem.Genre = model.Genre;
                existingItem.SubGenre = model.SubGenre;
                existingItem.LastUpdatedBy = User.Identity.Name;
                existingItem.LastUpdated = DateTime.Now;
                if (existingStocktake != null)
                {
                    existingStocktake.Quantity = model.StocktakeQuantity;
                    existingStocktake.Price = model.StocktakePrice;
                    //existingStocktake.SourceId = model.StocktakeSourceId;
                    _dbContext.SaveChanges();
                    return RedirectToAction("Details", "Product", new {id = existingItem.Id});
                }
            }
            Console.WriteLine("ERROR!! @ EditItem");
        }
        return View(model);
    }
    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost]
    public async Task<IActionResult> RemoveItem(int productId)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
        if (product != null)
        {
            _dbContext.Products.Remove(product);
            var stocktakes = _dbContext.Stocktakes.Where(s => s.ProductId == productId);
            _dbContext.Stocktakes.RemoveRange(stocktakes);
            await _dbContext.SaveChangesAsync();
        }
        return RedirectToAction("Index", "Product");
        
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
    public async Task<ProductViewModel> GetProductViewModelById(int id)
    {
        var productViewModel = _dbContext.Products
            .Include(p => p.GenreNavigation)
            .Include(p => p.Stocktakes).ThenInclude(s => s.Source)
            .Where(p => p.Id == id)
            .Select(p => ProductToViewModel(p)).First();
        return productViewModel;
    }

    private Product GetProductById(int id)
    {
        var product =  _dbContext.Products
            .Include(p => p.GenreNavigation)
            .Include(p => p.Stocktakes).ThenInclude(s => s.Source)
            .Where(p => p.Id == id)
            .Select(p =>(p)).First();
        return product;
    }
}