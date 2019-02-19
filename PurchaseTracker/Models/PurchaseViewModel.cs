using System;
namespace PurchaseTracker.Models
{
    public class PurchaseViewModel
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string Payee { get; set; }

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public string Memo { get; set; }
    }
}
