using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LapProject.Extensions;
using LapProject.Services;
using LapProject.Models.Cart;
using System.Web.Mvc;

namespace LapProject.Controllers
{
    [Authorize]
    public class CartController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            var dbCart = CartService.GetCartWithContent(CustomerEmail);

            var vm = new IndexVm
            {
                TotalPrice = dbCart.GrossTotalPrice.ToEuroString()
            };
            //Ab 10 Items beliebiger Zusammensetzung, wird ein 15% Rabatt auf die Gesamtsumme gewährt.
            int itemCount = 0;
            foreach (var orderLine in dbCart.OrderLines)
            {
                var product = ProductService.GetProduct(orderLine.ProductId);

                var vmOrderLine = new IndexVmOrderLine
                {
                    ProductId = product.ProductId,
                    Name = product.ProductName,
                    Manufacturer = product.ManufacturerName,
                    ImagePath = product.ImagePath,
                    GrossLinePrice = orderLine.GrossLinePrice.ToEuroString(),
                    GrossUnitPrice = orderLine.GrossUnitPrice.ToEuroString(),
                    Quantity = orderLine.Amount
                };
                itemCount += vmOrderLine.Quantity;

                vm.OrderLines.Add(vmOrderLine);
            }
            if(itemCount >= 10)
            {
                vm.TotalPrice = (dbCart.GrossTotalPrice * 0.85M).ToEuroString();
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult Add(int productId, int? quantity = 1, string returnUrl = "")
        {
            CartService.AddToCart(CustomerEmail, productId, quantity.Value);

            return SafeRedirect(returnUrl);
        }

        [HttpPost]
        public ActionResult Remove(int productId)
        {
            CartService.RemoveFromCart(CustomerEmail, productId);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public ActionResult ChangeQuantity(int productId, int quantity)
        {
            CartService.ChangeQuantity(CustomerEmail, productId, quantity);

            return RedirectToAction(nameof(Index));
        }

    }
}