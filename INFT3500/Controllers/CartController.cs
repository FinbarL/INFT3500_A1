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
    private ProductController ProductController;
    public CartController(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
        ProductController = new ProductController(_dbContext);
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
        var product = await ProductController.GetProductViewModelById(id);
        if (product == null)
        {
            Console.WriteLine("Product not found");
            return null;
        }
        var cart = SessionHelper.GetObjectFromSession<List<CartViewModel>>(HttpContext.Session, "cart");
        if (cart == null)
        {
            var productToAdd = new CartViewModel { Product = product, Quantity = 1 };
            cart = new List<CartViewModel> { productToAdd };
            SessionHelper.AddObjectToSession(HttpContext.Session, "cart", cart);
        }
        else
        {
            var existingItem = FindCartItemById(id);
            if (existingItem == null)
            {
                var productToAdd = new CartViewModel { Product = product, Quantity = 1 };
                cart.Add(productToAdd);
            }
            else
            {
                cart.Find(p => p.Product.ProductId == id).Quantity++;
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

        cart.RemoveAll(p => p.Product.ProductId == id);
        SessionHelper.AddObjectToSession(HttpContext.Session, "cart", cart);
        return RedirectToAction("Index", "Cart");
    }
    [Authorize]
    public async Task<IActionResult> DecrementQty(int id)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
        {
            Console.WriteLine("Product not found");
            return null;
        }

        var cart = SessionHelper.GetObjectFromSession<List<CartViewModel>>(HttpContext.Session, "cart");
        
            var existingItem = FindCartItemById(id);
            if (existingItem != null)
            {
                var cartItem = cart.Find(p => p.Product.ProductId == id);
                if(cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;
                }
                else
                {
                    cart.RemoveAll(p => p.Product.ProductId == id);
                }
            }
            SessionHelper.AddObjectToSession(HttpContext.Session, "cart", cart);

        return RedirectToAction("Index", "Cart");
    }

    [Authorize]
    public async Task<IActionResult> UpdateCart(CartViewModel cartItem)
    {
        Console.WriteLine("UPDATE CART");
        var cart = SessionHelper.GetObjectFromSession<List<CartViewModel>>(HttpContext.Session, "cart");
        if (cart == null)
        {
            Console.WriteLine("CART EMPTY!");
            return RedirectToAction("Index", "Cart");
        }

        // var existingItem = FindCartItemById(cartItem.Product.Id);
        if (cartItem.Quantity == 0)
        {
            await RemoveFromCart(cartItem.Product.ProductId);
        }
        else
        {
            var existingItem = FindCartItemById(cartItem.Product.ProductId);
            if (existingItem == null)
            {
                Console.WriteLine("CART ITEM NOT FOUND!");
            }
            else
            {
                cart.Find(p => p.Product.ProductId == cartItem.Product.ProductId).Quantity = cartItem.Quantity;
            }
        }
        SessionHelper.AddObjectToSession(HttpContext.Session, "cart", cart);
        return RedirectToAction("Index", "Cart");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var cart = SessionHelper.GetObjectFromSession<List<CartViewModel>>(HttpContext.Session, "cart");
        return View(cart);
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> FinalizeCheckout()
    {
        var userName = User.Identity.Name;
        var cart = SessionHelper.GetObjectFromSession<List<CartViewModel>>(HttpContext.Session, "cart");
        if (cart == null)
        {
            return RedirectToAction("Index", "Cart");
        }
        var userInfo = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        var order = new Order
        {
        };
        return RedirectToAction("Index", "Cart");
    }

    private CartViewModel FindCartItemById(int id)
    {
        var cart = SessionHelper.GetObjectFromSession<List<CartViewModel>>(HttpContext.Session, "cart");
        if (cart == null)
        {
            return null;
        }

        return cart.FirstOrDefault(p => p.Product.ProductId == id);
    }
}