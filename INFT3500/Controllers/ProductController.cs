using INFT3500.Models;
using INFT3500.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        var productList = new List<Product>();
        if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
        {
            productList = await _dbContext.Products
                .Include(p => p.GenreNavigation)
                .Include(p => p.Stocktakes).ThenInclude(s => s.Source)
                .Where(p => p.Author != null && p.Name != null &&
                            (searchString == null || searchString == "" ||
                             p.Name.Contains(searchString) || p.Author.Contains(searchString))).ToListAsync();
        }
        else
        {
            productList = await _dbContext.Products
                .Include(p => p.GenreNavigation)
                .Include(p => p.Stocktakes).ThenInclude(s => s.Source)
                .Where(p => p.Author != null && p.Name != null &&
                            (searchString == null || searchString == "" ||
                             p.Name.Contains(searchString) || p.Author.Contains(searchString)) &&
                            p.Stocktakes.Count(s => s.Source.ExternalLink != null) > 0).ToListAsync();
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
            product.Quantity -= productStocktake.Value;
        }

        return products;
    }

    [Authorize(Policy = "RequireAdminRole")]
    public IActionResult AddItem()
    {
        var model = new AddProductViewModel
        {
            Genre = 1,
            SubGenreList = GetSubGenreSelectList()
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
            return RedirectToAction("Details", "Product", new { id = newProduct.Id });
        }

        return View(model);
    }

    public IActionResult EditItem(int id)
    {
        Console.WriteLine("EditItem [GET] Called");
        var productViewModel = GetProductById(id);
        if (productViewModel == null)
        {
            Console.WriteLine("Product not found");
            return RedirectToAction("Index", "Product");
        }

        //logg all values of productViewModel
        Console.WriteLine("ID:" + productViewModel.Id);
        Console.WriteLine("Name:" + productViewModel.Name);
        Console.WriteLine("Author:" + productViewModel.Author);
        Console.WriteLine("Description:" + productViewModel.Description);
        Console.WriteLine("Published:" + productViewModel.Published);
        Console.WriteLine("Genre:" + productViewModel.Genre);
        Console.WriteLine("SubGenre:" + productViewModel.SubGenre);
        Console.WriteLine("LastUpdatedBy:" + productViewModel.LastUpdatedBy);
        Console.WriteLine("LastUpdated:" + productViewModel.LastUpdated);
        Console.WriteLine("StocktakeSourceId:" +
                          productViewModel.Stocktakes.FirstOrDefault(s => s.Source?.ExternalLink != null)?.SourceId);
        Console.WriteLine("StocktakeQuantity:" +
                          productViewModel.Stocktakes.FirstOrDefault(s => s.Source?.ExternalLink != null)?.Quantity);
        Console.WriteLine("StocktakePrice:" +
                          productViewModel.Stocktakes.FirstOrDefault(s => s.Source?.ExternalLink != null)?.Price);

        var addProductViewModel = new AddProductViewModel
        {
            Name = productViewModel.Name,
            Author = productViewModel.Author,
            Description = productViewModel.Description,
            Published = productViewModel.Published,
            Genre = productViewModel.Genre,
            SubGenre = productViewModel.SubGenre,
            SubGenreList = GetSubGenreSelectList(),
            Id = productViewModel.Id,
            RealQuantity = GetCurrentQtyLeft(productViewModel.Id),
            StocktakeSourceId = productViewModel.Stocktakes.FirstOrDefault(s => s.Source?.ExternalLink != null)?.SourceId ?? 0,
            StocktakeQuantity = productViewModel.Stocktakes.FirstOrDefault(s => s.Source?.ExternalLink != null)?.Quantity ?? 0,
            StocktakePrice = productViewModel.Stocktakes.FirstOrDefault(s => s.Source?.ExternalLink != null)?.Price ?? 0,
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
            var existingItem = _dbContext.Products.FirstOrDefault(p => p.Id == model.Id);
            var existingStocktake =
                _dbContext.Stocktakes.FirstOrDefault(s => s.Source != null && s.ProductId == model.Id && s.Source.ExternalLink != null);
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
            Price =
                product.Stocktakes.Where(s => s.Source.ExternalLink != null).Select(s => s.Price).FirstOrDefault() ?? 0,
        };
        return productViewModel;
    }

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

    public Product GetProductById(int id)
    {
        var product = _dbContext.Products
            .Include(p => p.GenreNavigation)
            .Include(p => p.Stocktakes).ThenInclude(s => s.Source)
            .Where(p => p.Id == id)
            .Select(p => (p)).First();
        return product;
    }

    public List<string?> GetSubGenreList()
    {
        var subGenres = _dbContext.Products.Select(p => p.SubGenre).Distinct().ToList();
        return subGenres;
    }
    public List<SelectListItem> GetSubGenreSelectList()
    {
        return GetSubGenreList().Select(sg => new SelectListItem
        {
            Value = sg,
            Text = sg
        }).ToList();
    }

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
