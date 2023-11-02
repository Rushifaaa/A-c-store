using LapProject.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LapProject.Models.Products;
using LapProject.Data;

namespace LapProject.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var dbProducts = ProductService.GetAllProducts();

            var first = (0, 0);
            var second = (0, 0);
            var third = (0, 0);
            var fourth = (0, 0);
            var fifth = (0, 0);

            //Ermittle die 5 Produkte mit den höchsten Verkaufszahlen und ordne sie anhand dieser an.
            foreach (var dbProduct in dbProducts)
            {
                int sellCount = ProductService.GetSellCount(dbProduct.ProductId);
                if (first.Item2 < sellCount)
                {
                    fifth = fourth;
                    fourth = third;
                    third = second;
                    second = first;
                    first = (dbProduct.ProductId, sellCount);
                }
                else if (second.Item2 < sellCount)
                {
                    fifth = fourth;
                    fourth = third;
                    third = second;
                    second = (dbProduct.ProductId, sellCount);
                }
                else if (third.Item2 < sellCount)
                {
                    fifth = fourth;
                    fourth = third;
                    third = (dbProduct.ProductId, sellCount);
                }
                else if (fourth.Item2 < sellCount)
                {
                    fifth = fourth;
                    fourth = (dbProduct.ProductId, sellCount);
                }
                else if (fifth.Item2 < sellCount)
                {
                    fifth = (dbProduct.ProductId, sellCount);
                }
            }

            var vm = new IndexVm()
            {
                ManufacturerDropdownData = ProductService.GetManufacturerDropdownDictionary(),
                CategoryDropdownData = ProductService.GetCategoryDropdownDictionary()
            };

            //Hole die Daten zu den bestverkauften Produkten. Falls weniger als 5 Produkte verkauft wurden, werden dementsprechend weniger angezeigt.
            for(int i = 1; i <= 5; i++)
            {

                ProductFull product = null;
                int tempSellcount = 0;
                if(i == 1 && first.Item1 != 0)
                {
                    product = ProductService.GetProduct(first.Item1);
                    tempSellcount = first.Item2;
                } else if(i == 2 && second.Item1 != 0)
                {
                    product = ProductService.GetProduct(second.Item1);
                    tempSellcount = second.Item2;
                } else if(i == 3 && third.Item1 != 0)
                {
                    product = ProductService.GetProduct(third.Item1);
                    tempSellcount = third.Item2;
                } else if(i == 4 && fourth.Item1 != 0)
                {
                    product = ProductService.GetProduct(fourth.Item1);
                    tempSellcount = fourth.Item2;
                } else if(i == 5 && fifth.Item1 != 0)
                {
                    product = ProductService.GetProduct(fifth.Item1);
                    tempSellcount = fifth.Item2;
                }
                else
                {
                    product = null;
                }
                if(product != null)
                {
                    var vmProduct = new IndexVmProduct
                    {
                        Id = product.ProductId,
                        Name = product.ProductName,
                        Manufacturer = product.ManufacturerName,
                        Price = product.GrossUnitPriceString,
                        ImagePath = product.ImagePath,
                        Sellcount = tempSellcount.ToString()
                    };
                    vm.Products.Add(vmProduct);
                }
            }

           

            return View(vm);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult AGBs()
        {
            return View();
        }
    }
}