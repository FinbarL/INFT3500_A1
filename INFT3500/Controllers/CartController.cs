using INFT3500.Helpers;
using INFT3500.Models;
using INFT3500.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace INFT3500.Controllers;
[Route("[controller]")]
public class CartController : Controller
{
    private readonly StoreDbContext _dbContext;
    private ProductController _productController;
    private AccountController _accountController;

    public CartController(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
        _productController = new ProductController(_dbContext);
        _accountController = new AccountController(_dbContext);
    }

    [Authorize]
    [HttpGet("[action]")]
    public async Task<IActionResult> Index()
    {
        var cart = SessionHelper.GetObjectFromSession<List<CartViewModel>>(HttpContext.Session, "cart");
        return View(cart);
    }

    [Authorize]
    [HttpPost("[action]")]
    public async Task<IActionResult> AddToCart(int productId)
    {
        Console.WriteLine("CALLED ADD TO CART");
        var product = await _productController.GetProductViewModelById(productId);
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
            var existingItem = FindCartItemById(productId);
            if (existingItem == null)
            {
                var productToAdd = new CartViewModel { Product = product, Quantity = 1 };
                cart.Add(productToAdd);
            }
            else
            {
                cart.Find(p => p.Product.ProductId == productId).Quantity++;
            }

            SessionHelper.AddObjectToSession(HttpContext.Session, "cart", cart);
        }

        return RedirectToAction("Index", "Cart");
    }

    [Authorize]
    [HttpPost("[action]")]
    public async Task<IActionResult> RemoveFromCart(int productId)
    {
        var cart = SessionHelper.GetObjectFromSession<List<CartViewModel>>(HttpContext.Session, "cart");
        if (cart == null)
        {
            return RedirectToAction("Index", "Cart");
        }

        cart.RemoveAll(p => p.Product.ProductId == productId);
        SessionHelper.AddObjectToSession(HttpContext.Session, "cart", cart);
        return RedirectToAction("Index", "Cart");
    }

    [Authorize]
    [HttpPost("[action]")]
    public async Task<IActionResult> DecrementQty(int productId, int amountRemoved = 1)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
        if (product == null)
        {
            Console.WriteLine("Product not found");
            return RedirectToAction("Index", "Cart");
        }

        var cart = SessionHelper.GetObjectFromSession<List<CartViewModel>>(HttpContext.Session, "cart");

        var existingItem = FindCartItemById(productId);
        if (existingItem != null)
        {
            var cartItem = cart.Find(p => p.Product.ProductId == productId);
            if (cartItem.Quantity > amountRemoved)
            {
                cartItem.Quantity -= amountRemoved;
            }
            else
            {
                cart.RemoveAll(p => p.Product.ProductId == productId);
            }
        }

        SessionHelper.AddObjectToSession(HttpContext.Session, "cart", cart);

        return RedirectToAction("Index", "Cart");
    }

    [Authorize]
    [HttpPost("[action]")]
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
    [HttpGet("[action]")]
    public async Task<IActionResult> Checkout()
    {
            Console.WriteLine("CALLED [GET] CHECKOUT!!!");
            var userName = User.Identity.Name;
            var cart = SessionHelper.GetObjectFromSession<List<CartViewModel>>(HttpContext.Session, "cart");
            if (cart == null)
            {
                return RedirectToAction("Index", "Cart");
            }

            var billingInfo = _accountController.GetUserViewModel(userName);
            List<CartViewModel> lowStockItems = new List<CartViewModel>();
            foreach (var cartItem in cart)
            {
                var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == cartItem.Product.ProductId);
                var stocktake = await _dbContext.Stocktakes.FirstOrDefaultAsync(s => s.ProductId == product.Id);
                if (product == null || stocktake == null)
                {
                    Console.WriteLine("Product or stocktake not found");
                    ViewBag.ErrorMessage = "Product or stocktake not found" + cartItem.Product.ProductId;
                    return View();
                }

                var qtyLeft = _productController.GetCurrentQtyLeft(cartItem.Product.ProductId);
                var qtyToOrder = cartItem.Quantity;
                Console.WriteLine("QTYLEFT:" + qtyLeft);
                if (qtyLeft < qtyToOrder)
                {
                    lowStockItems.Add(cartItem);
                }
            }

            //check if any items are low on stock
            if (lowStockItems.Count > 0)
            {
                var errorMessage = "The following items were out of stock: ";
                foreach (var lowStockItem in lowStockItems)
                {
                    var currentQtyLeft = _productController.GetCurrentQtyLeft(lowStockItem.Product.ProductId);
                    var qtySelected = lowStockItem.Quantity;
                    errorMessage += lowStockItem.Product.Name + " (Qty Left: " + currentQtyLeft + "), ";
                    await DecrementQty(lowStockItem.Product.ProductId, qtySelected - currentQtyLeft);
                    cart = SessionHelper.GetObjectFromSession<List<CartViewModel>>(HttpContext.Session, "cart");
                }

                errorMessage += "Your cart has been automatically adjusted";
                ViewBag.ErrorMessage = errorMessage;
                Console.WriteLine("No stock left");
            }

            var cartPageViewModel = new CartPageViewModel
            {
                Products = cart,
                User = billingInfo,
            };
            return View(cartPageViewModel);
    }

    [Authorize]
    [HttpPost("[action]")]
    public async Task<IActionResult> Checkout(CartPageViewModel cartPageViewModel)
    {
        if (ModelState.IsValid)
        {
            var userName = User.Identity.Name;
            var newToData = _dbContext.Tos.FirstOrDefault(u => u.UserName == userName);
            //if a to exists, update it, else create a new one. use the viewmodel data
            if (newToData != null)
            {
                newToData.StreetAddress = cartPageViewModel.User.Address;
                newToData.Suburb = cartPageViewModel.User.Suburb;
                newToData.State = cartPageViewModel.User.State;
                newToData.PostCode = Convert.ToInt32(cartPageViewModel.User.PostCode);
                newToData.PhoneNumber = cartPageViewModel.User.PhoneNumber;
                newToData.Email = cartPageViewModel.User.BillingEmail;
                _dbContext.Tos.Update(newToData);
            }
            else
            {
                var newTo = new To
                {
                    UserName = userName,
                    StreetAddress = cartPageViewModel.User.Address,
                    Suburb = cartPageViewModel.User.Suburb,
                    State = cartPageViewModel.User.State,
                    PostCode = Convert.ToInt32(cartPageViewModel.User.PostCode),
                    PhoneNumber = cartPageViewModel.User.PhoneNumber,
                    Email = cartPageViewModel.User.EmailAddress,
                };
                _dbContext.Tos.Add(newTo);
            }

            await _dbContext.SaveChangesAsync();

            var userToInfo = _dbContext.Tos.FirstOrDefault(u => u.UserName == userName);
            var order = new Order
            {
                Customer = userToInfo.CustomerId,
                StreetAddress = userToInfo.StreetAddress,
                PostCode = Convert.ToInt32(userToInfo.PostCode),
                Suburb = userToInfo.Suburb,
                State = userToInfo.State,
            };
            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();
            Console.WriteLine(order.OrderId);
            var productsInOrders = new List<ProductsInOrder>();
            var cart = SessionHelper.GetObjectFromSession<List<CartViewModel>>(HttpContext.Session, "cart");
            foreach (var cartItem in cart)
            {
                var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == cartItem.Product.ProductId);
                var stocktake = await _dbContext.Stocktakes.FirstOrDefaultAsync(s => s.ProductId == product.Id);
                if (product == null || stocktake == null)
                {
                    Console.WriteLine("Product or stocktake not found");
                    ViewBag.ErrorMessage = "Product or stocktake not found";
                    return View(cartPageViewModel);
                }
                var productInOrder = new ProductsInOrder
                {
                    OrderId = order.OrderId,
                    ProduktId = stocktake.ItemId,
                    Quantity = cartItem.Quantity,
                };
                productsInOrders.Add(productInOrder);
            }

            foreach (ProductsInOrder productInOrder in productsInOrders)
            {
                Console.WriteLine(productInOrder.OrderId + " " + productInOrder.ProduktId + " " +
                                  productInOrder.Quantity);
                var insertSql = $"" +
                                $"INSERT INTO ProductsInOrders (OrderId, ProduktId, Quantity) " +
                                $"VALUES ({productInOrder.OrderId}, {productInOrder.ProduktId}, {productInOrder.Quantity})";
                await _dbContext.Database.ExecuteSqlRawAsync(insertSql);
            }

            await _dbContext.SaveChangesAsync();
            ClearCart();
            return RedirectToAction("Index", "Cart");
        } else
        {
            var cart = SessionHelper.GetObjectFromSession<List<CartViewModel>>(HttpContext.Session, "cart");
            cartPageViewModel.Products = cart;
            Console.WriteLine("ERROR!");
            foreach(var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }
        }

        return View(cartPageViewModel);
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

    private void ClearCart()
    {
        var cart = SessionHelper.GetObjectFromSession<List<CartViewModel>>(HttpContext.Session, "cart");
        if (cart != null)
        {
            cart.Clear();
            SessionHelper.AddObjectToSession(HttpContext.Session, "cart", cart);
            Console.WriteLine("Cart Cleared!");
        }
    }

}
