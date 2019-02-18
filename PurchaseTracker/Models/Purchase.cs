using System;
namespace PurchaseTracker.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Payee { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; }
        public string Memo { get; set; }
    }
}
