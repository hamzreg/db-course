using Microsoft.EntityFrameworkCore;

using WineSales.Domain.Models;

namespace WineSales.Data
{
    public class DataBaseContext : DbContext
    {
        public DbSet<BonusCard> BonusCards { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SupplierWine> SupplierWines { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Wine> Wines { get; set; }


        public DataBaseContext(DbContextOptions<DataBaseContext> options)
            : base(options) { }
    }
}
