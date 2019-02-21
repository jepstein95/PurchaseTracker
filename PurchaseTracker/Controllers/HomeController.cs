using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Kendo.Mvc.UI;
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

        // Category list caches
        private Dictionary<int, string> _categoryMap;
        private List<CategoryViewModel> _categoryViewModels;

        public HomeController(PurchasesDbContext context)
        {
            _context = context;

            // Maintain a cache of the category list so we don't have to
            // hit the database every time we need to look one up
            _categoryMap = new Dictionary<int, string>();
            _categoryViewModels = new List<CategoryViewModel>();
            foreach (Category category in _context.Categories)
            {
                _categoryMap.Add(category.Id, category.Name);
                _categoryViewModels.Add(new CategoryViewModel() {
                    Id = category.Id,
                    Name = category.Name
                });
            }
        }

        // Index view
        public ActionResult Index()
        {
            return View();
        }

        // Error view
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public ActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // API: GET /Home/GetCategories
        [HttpGet]
        public ActionResult GetCategories()
        {
            return Json(_categoryViewModels);
        }


        // API: GET /Home/GetPurchases
        [HttpGet]
        public ActionResult GetPurchases()
        {
            List<PurchaseViewModel> viewModels = new List<PurchaseViewModel>();
            try
            {
                foreach (Purchase purchase in _context.Purchases)
                {
                    viewModels.Add(new PurchaseViewModel()
                    {
                        Id = purchase.Id,
                        CategoryId = purchase.CategoryId,
                        CategoryName = _categoryMap.GetValueOrDefault(purchase.CategoryId),
                        Payee = purchase.Payee,
                        Amount = purchase.Amount,
                        Date = purchase.Date,
                        Memo = purchase.Memo
                    });
                }
            }
            catch
            {
                return Json(new DataSourceResult() { Errors = "Could not connect to the database." });
            }
            return Json(viewModels);
        }

        // API: POST /Home/UpdatePurchase
        [HttpPost]
        public ActionResult UpdatePurchase()
        {
            if (!TryGetPurchaseViewModel(out PurchaseViewModel model))
            {
                return Json(new DataSourceResult() { Errors = "Model could not be deserialized." });
            }

            try
            {
                Purchase purchase = _context.Purchases.Find(model.Id);
                UpdatePurchaseModel(ref purchase, ref model);
                _context.SaveChanges();
            }
            catch(Exception)
            {
                return Json(new DataSourceResult() { Errors = "Could not connect to the database." });
            }

            return Json(model);
        }

        // API: POST /Home/DestroyPurchase
        [HttpPost]
        public ActionResult DestroyPurchase()
        {
            if (!TryGetPurchaseViewModel(out PurchaseViewModel model))
            {
                return Json(new DataSourceResult() { Errors = "Model could not be deserialized." });
            }

            try
            {
                Purchase purchase = _context.Purchases.Find(model.Id);
                _context.Purchases.Remove(purchase);
                _context.SaveChanges();
            }
            catch(Exception)
            {
                return Json(new DataSourceResult() { Errors = "Could not connect to the database." });
            }

            return Json(model);
        }

        // API: POST /Home/CreatePurchase
        [HttpPost]
        public ActionResult CreatePurchase()
        {
            if (!TryGetPurchaseViewModel(out PurchaseViewModel model))
            {
                return Json(new DataSourceResult() { Errors = "Model could not be deserialized." });
            }
            try
            {
                Purchase purchase = new Purchase();
                UpdatePurchaseModel(ref purchase, ref model);
                _context.Purchases.Add(purchase);
                _context.SaveChanges();
                model.Id = purchase.Id;
            }
            catch(Exception)
            {
                return Json(new DataSourceResult() { Errors = "Could not connect to the database." });
            }

            return Json(model);
        }

        // Deserialize the request string to an instance of PurchaseViewModel
        private bool TryGetPurchaseViewModel(out PurchaseViewModel model)
        {
            try
            {
                StringValues modelsString = Request.Form["models"];
                model = JsonConvert.DeserializeObject<List<PurchaseViewModel>>(modelsString).First();
            }
            catch(Exception)
            {
                model = new PurchaseViewModel();
                return false;
            }
            return true;
        }

        // Update a Purchase from a PurchaseViewModel
        private void UpdatePurchaseModel(ref Purchase p, ref PurchaseViewModel model)
        {
            p.CategoryId = model.CategoryId;
            p.Payee = model.Payee;
            p.Amount = model.Amount;
            p.Memo = model.Memo;
            p.Date = model.Date;
            model.CategoryName = _categoryMap.GetValueOrDefault(model.CategoryId);
        }

    }
}
