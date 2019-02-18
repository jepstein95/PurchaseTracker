using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using PurchaseTracker.Models;

namespace PurchaseTracker.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Purchases_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<Purchase> purchases = new List<Purchase>();
            Purchase p = new Purchase();
            p.Amount = 10;
            p.Payee = "Me";
            p.Date = new DateTime();
            p.CategoryId = 1;
            purchases.Add(p);
            return Json(purchases.ToDataSourceResult(request, ModelState));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
