using System;
using Microsoft.EntityFrameworkCore;
using PurchaseTracker.Models;

namespace PurchaseTracker.Services
{
    public class PurchasesDbContext : DbContext
    {
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Category> Categories { get; set; }

        public PurchasesDbContext(DbContextOptions<PurchasesDbContext> options)
               : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
