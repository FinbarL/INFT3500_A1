using INFT3500.Models;
using INFT3500.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace INFT3500.Controllers;

using Microsoft.AspNetCore.Mvc;

[Route("[controller]")]
public class ProductController : Controller
{
    private readonly StoreDbContext _dbContext;

    public ProductController(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Index()
    {
        var productViewModels = await GetProductList(null);
        return View(productViewModels);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Index(string searchString)
    {
        var productViewModels = await GetProductList(searchString);
        return View(productViewModels);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Details(int id)
    {
        var productViewModel = await GetProductViewModelById(id);
        return View(productViewModel);
    }


    private async Task<List<ProductViewModel>> GetProductList(string? searchString)
    {
        var productList = new List<Product>();
        if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
        {
            productList = await _dbContext.Products
                .Include(p => p.GenreNavigation)
                .Include(p => p.Stocktakes).ThenInclude(s => s.Source)
                .Where(p => p.GenreNavigation != null && p.GenreNavigation.Name != null && p.Author != null &&
                            p.Name != null && p.Genre != null &&
                            (searchString == null || searchString == "" ||
                             p.Name.Contains(searchString) || p.GenreNavigation.Name.Contains(searchString) ||
                             p.Author.Contains(searchString))).ToListAsync();
        }
        else
        {
            productList = await _dbContext.Products
                .Include(p => p.GenreNavigation)
                .Include(p => p.Stocktakes).ThenInclude(s => s.Source)
                .Where(p => p.GenreNavigation != null && p.GenreNavigation.Name != null && p.GenreNavigation != null &&
                            p.Author != null && p.Name != null && p.Genre != null &&
                            (searchString == null || searchString == "" ||
                             p.Name.Contains(searchString) || p.GenreNavigation.Name.Contains(searchString) ||
                             p.Author.Contains(searchString)) &&
                            p.Stocktakes.Count(s => s.Source != null && s.Source.ExternalLink != null) > 0)
                .ToListAsync();
        }

        var productStocktakeList = new List<KeyValuePair<int, int>>();
        foreach (Product product in productList)
        {
            var stocktake = product.Stocktakes.FirstOrDefault();
            var productsInOrderCount = _dbContext.ProductsInOrders.Where(p => p.ProduktId == stocktake.ItemId)
                .Sum(pio => pio.Quantity);
            if (productsInOrderCount > 0)
            {
                productStocktakeList.Add(new KeyValuePair<int, int>(product.Id, productsInOrderCount ?? 0));
                Console.WriteLine("PRODUCTID=" + product.Id);
            }
        }

        var products = productList.Select(ProductToViewModel).ToList();
        foreach (var productStocktake in productStocktakeList)
        {
            var product = products.First(p => p.ProductId == productStocktake.Key);
            var productQty = Math.Min(0, product.Quantity - productStocktake.Value);
            product.Quantity = productQty;
        }
        if (User.Identity.IsAuthenticated && !User.IsInRole("Admin") && !User.IsInRole("Staff"))
        {
            products = products.Where(p => p.Quantity > 0).ToList();
        }
        return products;
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("[action]")]
    public IActionResult AddItem()
    {
        Console.WriteLine("Called");
        var model = new AddProductViewModel
        {
            Genre = 1,
            StocktakeSourceId = 1,
        };
        return View(model);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("[action]")]
    public IActionResult AddItem(AddProductViewModel model)
    {
        IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
        Console.WriteLine(allErrors);

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
            return RedirectToAction("Details", "Product", new { id = newProduct.Id });
        }

        return View(model);
    }

    [HttpGet("[action]")]
    public IActionResult EditItem(int id)
    {
        IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
        Console.WriteLine(allErrors);
        Console.WriteLine("EditItem [GET] Called");
        var productViewModel = GetProductById(id);
        if (productViewModel == null)
        {
            Console.WriteLine("Product not found");
            return RedirectToAction("Index", "Product");
        }

        var addProductViewModel = new AddProductViewModel
        {
            Name = productViewModel.Name,
            Author = productViewModel.Author,
            Description = productViewModel.Description,
            Published = productViewModel.Published,
            Genre = productViewModel.Genre,
            SubGenre = productViewModel.SubGenre,
            Id = productViewModel.Id,
            RealQuantity = GetCurrentQtyLeft(productViewModel.Id),
            StocktakeSourceId =
                productViewModel.Stocktakes.FirstOrDefault(s => s.Source?.ExternalLink != null)?.SourceId ?? 0,
            StocktakeQuantity =
                productViewModel.Stocktakes.FirstOrDefault(s => s.Source?.ExternalLink != null)?.Quantity ?? 0,
            StocktakePrice =
                productViewModel.Stocktakes.FirstOrDefault(s => s.Source?.ExternalLink != null)?.Price ?? 0,
        };
        return View(addProductViewModel);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("[action]")]
    public async Task<IActionResult> EditItem(AddProductViewModel model)
    {
        IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
        foreach (var error in allErrors)
        {
            Console.WriteLine("ERROR:" + error);
        }

        Console.WriteLine("EditItem [GET] Called");
        if (ModelState.IsValid)
        {
            Console.WriteLine("MODELID=" + model.Id);
            var existingItem = _dbContext.Products.FirstOrDefault(p => p.Id == model.Id);
            var existingStocktake =
                _dbContext.Stocktakes.FirstOrDefault(s =>
                    s.Source != null && s.ProductId == model.Id && s.Source.ExternalLink != null);
            Console.WriteLine("NEWSOURCE=" + model.StocktakeSourceId);
            Console.WriteLine("SOURCEID =" + existingStocktake?.SourceId);
            if (existingItem != null)
            {
                existingItem.Name = model.Name;
                existingItem.Author = model.Author;
                existingItem.Description = model.Description;
                existingItem.Published = model.Published;
                existingItem.Genre = model.Genre;
                existingItem.SubGenre = model.SubGenre;
                existingItem.LastUpdatedBy = User.Identity?.Name;
                existingItem.LastUpdated = DateTime.Now;
                if (existingStocktake != null)
                {
                    existingStocktake.Quantity = model.StocktakeQuantity;
                    existingStocktake.Price = model.StocktakePrice;
                    existingStocktake.SourceId = model.StocktakeSourceId;
                    await _dbContext.SaveChangesAsync();
                    return RedirectToAction("Details", "Product", new { id = existingItem.Id });
                }
                else
                {
                    var newStocktake = new Stocktake
                    {
                        ProductId = existingItem.Id,
                        Source = _dbContext.Sources.First(s => s.ExternalLink != null && s.Genre == model.Genre),
                        Quantity = model.StocktakeQuantity,
                        Price = model.StocktakePrice,
                    };
                    _dbContext.Stocktakes.Add(newStocktake);
                    await _dbContext.SaveChangesAsync();
                    return RedirectToAction("Details", "Product", new { id = existingItem.Id });
                }
            }

            Console.WriteLine("ERROR!! @ EditItem");
        }

        return View(model);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("[action]")]
    public async Task<IActionResult> RemoveItem(int productId)
    {
        var product = await _dbContext.Products
            .Include(p => p.GenreNavigation)
            .Include(p => p.Stocktakes).ThenInclude(s => s.Source)
            .Where(p => p.Id == productId).FirstOrDefaultAsync();
        if (product != null)
        {
            var stocktake = product.Stocktakes.FirstOrDefault();
            var productsInOrderCount = _dbContext.ProductsInOrders.Where(p => p.ProduktId == stocktake.ItemId)
                .Sum(pio => pio.Quantity);
            if(productsInOrderCount > 0)
            {
                TempData["ErrorMessage"] = "Cannot delete product with orders! If you wish to remove this product from user view, please reduce the quantity to 0.";
                return RedirectToAction("EditItem", "Product", new { id = productId });
            }
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
            Price =
                product.Stocktakes.Where(s => s.Source.ExternalLink != null).Select(s => s.Price).FirstOrDefault() ?? 0,
        };
        return productViewModel;
    }

    [NonAction]
    public async Task<ProductViewModel> GetProductViewModelById(int id)
    {
        var product = await _dbContext.Products
            .Include(p => p.GenreNavigation)
            .Include(p => p.Stocktakes).ThenInclude(s => s.Source)
            .Where(p => p.Id == id).FirstOrDefaultAsync();
        var stocktake = product.Stocktakes.FirstOrDefault();
        var productsInOrderCount = _dbContext.ProductsInOrders.Where(p => p.ProduktId == stocktake.ItemId)
            .Sum(pio => pio.Quantity);
        var productViewModel = ProductToViewModel(product);
        if (productsInOrderCount > 0)
        {
            productViewModel.Quantity -= productsInOrderCount ?? 0;
        }

        return productViewModel;
    }

    private Product GetProductById(int id)
    {
        var product = _dbContext.Products
            .Include(p => p.GenreNavigation)
            .Include(p => p.Stocktakes).ThenInclude(s => s.Source)
            .Where(p => p.Id == id)
            .Select(p => (p)).First();
        return product;
    }

    [HttpGet("[action]")]
    public int GetCurrentQtyLeft(int id)
    {
        var product = GetProductById(id);
        if (product == null)
        {
            Console.WriteLine("Product not found");
            return 0;
        }

        var stocktake = product.Stocktakes.FirstOrDefault();
        if (stocktake == null)
        {
            Console.WriteLine("Stocktake not found");
            return 0;
        }

        var productsInOrderCount = _dbContext.ProductsInOrders.Where(p => p.ProduktId == stocktake.ItemId)
            .Sum(pio => pio.Quantity);
        var stocktakeQty = product.Stocktakes.Where(s => s.Source.ExternalLink != null).Select(s => s.Quantity).Sum() ??
                           0;
        Console.WriteLine("StocktakeQty" + stocktakeQty);
        Console.WriteLine("ProductsInOrderCount" + productsInOrderCount);
        return (int)(stocktakeQty - productsInOrderCount);
    }
}