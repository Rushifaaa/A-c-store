using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LapProject.Extensions;
using LapProject.Services;
using LapProject.Models.Order;
using System.Web.Mvc;
using LapProject.Data;
using System.Data.Entity;

namespace LapProject.Controllers
{
    [Authorize]
    public class OrderController : BaseController
    {
        [HttpGet]
        public ActionResult New()
        {
            var dbCart = CartService.GetCartWithContent(CustomerEmail);

            var vm = new NewVm
            {
                FirstName = dbCart.FirstName,
                LastName = dbCart.LastName,
                Street = dbCart.Street,
                ZipCode = dbCart.ZipCode,
                City = dbCart.City,
                TotalPrice = dbCart.GrossTotalPrice.ToEuroString()
            };
            
            foreach (var orderLine in dbCart.OrderLines)
            {
                var product = ProductService.GetProduct(orderLine.ProductId);
                var vmOrderLine = new NewVmOrderLine
                {
                    ProductId = product.ProductId,
                    Name = product.ProductName,
                    Manufacturer = product.ManufacturerName,
                    GrossLinePrice = orderLine.GrossLinePrice.ToEuroString(),
                    GrossUnitPrice = orderLine.GrossUnitPrice.ToEuroString(),
                    Quantity = orderLine.Amount
                };

                vm.OrderLines.Add(vmOrderLine);
            }

            return View(vm);
        }

        //Hat sich der user für Überweisung entschieden, so wird er an die zugehörige Seite weitergeleitet.
        //Hat sich der user für Kreditkarte entscheiden, so wird er zu einer Seite weitergeleited, bei der er seine Zahlungsdaten angeben kann. Daraufhin wird er zu einer 'Danke für Ihre Bestellung'-Seite weitergeleitet.
        [HttpPost]
        public ActionResult Complete(CompleteVm cVm)
        {
            var orderId = CartService.OrderCart(CustomerEmail, cVm.FirstName, cVm.LastName, cVm.Street, cVm.ZipCode, cVm.City);
            switch (cVm.PaymentType)
            {
                case "Kreditkarte":
                    return RedirectToAction(nameof(CompletedCreditCard), new { id = orderId });
                default:
                case "Überweisung":
                    return RedirectToAction(nameof(Completed), new { id = orderId });
            }
        }

        [HttpGet]
        public ActionResult Completed(int id)
        {
            var order = OrderService.GetCustomerOrder(CustomerEmail, id);

            var vm = new CompletedVm
            {
                OrderId = order.Id,
                TotalPrice = order.PriceTotal.ToEuroString()
            };

            return View(nameof(Completed), vm);
        }
        [HttpGet]
        public ActionResult CompletedCreditCard(int id)
        {
            var order = OrderService.GetCustomerOrder(CustomerEmail,id);

            var vm = new CompletedVm
            {
                OrderId = order.Id,
                TotalPrice = order.PriceTotal.ToEuroString()
            };

            return View(nameof(CompletedCreditCard),vm);
        }
        [HttpGet]
        public ActionResult CompletedCreditCardFinished()
        {

            return View();
        }
    }
}