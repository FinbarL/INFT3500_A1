using INFT3500.Helpers;
using INFT3500.Models;
using INFT3500.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var cart = SessionHelper.GetObjectFromSession<List<CartViewModel>>(HttpContext.Session, "cart");
        return View(cart);
    }
    [Authorize]
    public async Task<IActionResult> AddToCart(int id)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
        {
            Console.WriteLine("Product not found");
            return null;
        }

        var cart = SessionHelper.GetObjectFromSession<List<CartViewModel>>(HttpContext.Session, "cart");
        if (cart == null)
        {
            cart = new List<CartViewModel> { new CartViewModel { Product = product, Quantity = 1 } };
            SessionHelper.AddObjectToSession(HttpContext.Session, "cart", cart);
        }
        else
        {
            var existingItem = FindCartItemById(id);
            if (existingItem == null)
            {
                cart.Add(new CartViewModel { Product = product, Quantity = 1 });
            }
            else
            {
                cart.Find(p => p.Product.Id == id).Quantity++;
            }
            SessionHelper.AddObjectToSession(HttpContext.Session, "cart", cart);
        }

        return RedirectToAction("Index", "Cart");
    }
    [Authorize]
    public async Task<IActionResult> RemoveFromCart(int id)
    {
        var cart = SessionHelper.GetObjectFromSession<List<CartViewModel>>(HttpContext.Session, "cart");
        if (cart == null)
        {
            return RedirectToAction("Index", "Cart");
        }

        cart.RemoveAll(p => p.Product.Id == id);
        SessionHelper.AddObjectToSession(HttpContext.Session, "cart", cart);
        return RedirectToAction("Index", "Cart");
    }
    [Authorize]
    public async Task<IActionResult> Checkout()
    {
        var cart = SessionHelper.GetObjectFromSession<List<CartViewModel>>(HttpContext.Session, "cart");
        return View(cart);
    }

    private CartViewModel FindCartItemById(int id)
    {
        var cart = SessionHelper.GetObjectFromSession<List<CartViewModel>>(HttpContext.Session, "cart");
        if (cart == null)
        {
            return null;
        }
        return cart.FirstOrDefault(p => p.Product.Id == id);
    }
}