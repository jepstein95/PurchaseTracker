using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using PurchaseTracker.Models;
using PurchaseTracker.Services;

namespace PurchaseTracker.Controllers
{
    public class HomeController : Controller
    {
        // Database context
        private readonly PurchasesDbContext _context;

        // Category list
        private Dictionary<int, string> _categories;

        public HomeController(PurchasesDbContext context)
        {
            _context = context;

            // Maintain a cache of the category list so we don't have to hit the
            // database every time we need to look up a category name
            _categories = new Dictionary<int, string>();
            foreach (Category category in _context.Categories)
            {
                _categories.Add(category.Id, category.Name);
            }
        }

        // Index view
        public IActionResult Index()
        {
            return View();
        }

        // Error view
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // API: GET /Home/GetCategories
        [HttpGet]
        public ActionResult GetCategories()
        {
            var categoryList = new List<Object>();
            foreach (KeyValuePair<int, string> category in _categories)
            {
                categoryList.Add(new { Id = category.Key, Name = category.Value });
            }
            return Json(categoryList);
        }


        // API: GET /Home/GetPurchases
        [HttpGet]
        public ActionResult GetPurchases()
        {
            List<Purchase> purchases = _context.Purchases.ToList();
            List<PurchaseViewModel> viewModels = new List<PurchaseViewModel>();
            foreach(var p in purchases)
            {
                viewModels.Add(new PurchaseViewModel()
                {
                    Id = p.Id,
                    CategoryId = p.CategoryId,
                    CategoryName = _categories.GetValueOrDefault(p.CategoryId),
                    Payee = p.Payee,
                    Amount = p.Amount,
                    Date = p.Date,
                    Memo = p.Memo
                });
            }
            return Json(viewModels);
        }

        // API: POST /Home/UpdatePurchase
        [HttpPost]
        public ActionResult UpdatePurchase()
        {
            PurchaseViewModel model = GetPurchaseViewModel();
            Purchase purchase = _context.Purchases.Find(model.Id);
            UpdatePurchase(ref purchase, ref model);
            _context.SaveChanges();
            return Json(model);
        }

        // API: POST /Home/DestroyPurchase
        [HttpPost]
        public ActionResult DestroyPurchase()
        {
            PurchaseViewModel model = GetPurchaseViewModel();
            Purchase purchase = _context.Purchases.Find(model.Id);
            _context.Purchases.Remove(purchase);
            _context.SaveChanges();
            return Json(model);
        }

        // API: POST /Home/CreatePurchase
        [HttpPost]
        public ActionResult CreatePurchase()
        {
            PurchaseViewModel model = GetPurchaseViewModel();
            Purchase purchase = new Purchase();
            UpdatePurchase(ref purchase, ref model);
            _context.Purchases.Add(purchase);
            _context.SaveChanges();
            return Json(model);
        }

        // Deserialize the request string to an instance of PurchaseViewModel
        private PurchaseViewModel GetPurchaseViewModel()
        {
            StringValues modelsString = Request.Form["models"];
            return JsonConvert.DeserializeObject<List<PurchaseViewModel>>(modelsString).First();
        }

        // Update a Purchase from a PurchaseViewModel
        private void UpdatePurchase(ref Purchase p, ref PurchaseViewModel model)
        {
            p.CategoryId = model.CategoryId;
            p.Payee = model.Payee;
            p.Amount = model.Amount;
            p.Memo = model.Memo;
            p.Date = model.Date;
            model.CategoryName = _categories.GetValueOrDefault(model.CategoryId);
        }

    }
}
