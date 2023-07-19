using INFT3500.Helpers;
using INFT3500.Models;
using INFT3500.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace INFT3500.Controllers;

public class CartController : Controller 
{
    private readonly StoreDbContext _dbContext;

    public CartController(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        var cart = SessionHelper.GetObjectFromSession<List<Product>>(HttpContext.Session, "cart");
        return View(cart);
    }
    
    public async Task<IActionResult> AddToCart(int id)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
        {
            Console.WriteLine("Product not found");
            return null;
        }
        var cart = SessionHelper.GetObjectFromSession<List<Product>>(HttpContext.Session, "cart");
        if (cart == null)
        {
            cart = new List<Product> { product };
            SessionHelper.AddObjectToSession(HttpContext.Session, "cart", cart);
        }
        else
        {
            cart.Add(product);
            SessionHelper.AddObjectToSession(HttpContext.Session, "cart", cart);
        }
        Console.WriteLine("Called!");
        return RedirectToAction("Index", "Cart");
    }
    public async Task<IActionResult> RemoveFromCart(int id)
    {
        var cart = SessionHelper.GetObjectFromSession<List<Product>>(HttpContext.Session, "cart");
        if (cart == null)
        {
            return RedirectToAction("Index", "Cart");
        }
        else
        {
            var productToRemove = cart.FirstOrDefault(p => p.Id == id);
            if (productToRemove == null)
            {
                Console.WriteLine($"No product matching id={id} found in cart");
            }
            cart.Remove(productToRemove);
            SessionHelper.AddObjectToSession(HttpContext.Session, "cart", cart);
        }
        return RedirectToAction("Index", "Cart");
    }
    
    public async Task<IActionResult> Checkout()
    {
        
        return View();
    }
}