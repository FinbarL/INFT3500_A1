using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using INFT3500.Controllers;
using INFT3500.Helpers;
using INFT3500.ViewModels;

namespace INFT3500.ViewComponents;

[ViewComponent(Name = "CartTotal")]
public class CartTotalViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        return View("_CartTotal", GetCartTotal());
    }
    private int GetCartTotal()
    {
        var cart = SessionHelper.GetObjectFromSession<List<CartViewModel>>(HttpContext.Session, "cart");
        if (cart == null)
        {
            return 0;
        }
        return cart.Sum(p => p.Quantity);
    }
}