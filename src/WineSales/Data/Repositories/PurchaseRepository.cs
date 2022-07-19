using WineSales.Domain.Models;
using WineSales.Domain.RepositoryInterfaces;
using WineSales.Domain.Exceptions;

namespace WineSales.Data.Repositories
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly DataBaseContext _context;

        public PurchaseRepository(DataBaseContext context)
        {
            _context = context;
        }

        public void Create(Purchase purchase)
        {
            try
            {
                _context.Purchases.Add(purchase);
                _context.SaveChanges();
            }
            catch
            {
                throw new PurchaseException("Failed to create purchase.");
            }
        }

        public List<Purchase> GetAll()
        {
            return _context.Purchases.ToList();
        }

        public Purchase GetByID(int id)
        {
            return _context.Purchases.Find(id);
        }

        public List<Purchase> GetByPrice(double price)
        {
            return _context.Purchases.Where(purchase => purchase.Price == price)
                .ToList();
        }

        public List<Purchase> GetByStatus(int status)
        {
            return _context.Purchases.Where(purchase => purchase.Status == status)
                .ToList();
        }

        public (List<Wine>, List<double>) GetByCustomerID(int customerID)
        {
            var purchases = _context.Purchases.Where(purchase => purchase.CustomerID == customerID)
                .ToList();

            var wines = new List<Wine>();
            var prices = new List<double>();

            foreach (Purchase purchase in purchases)
            {
                var sales = _context.Sales.Where(sl => sl.PurchaseID == purchase.ID)
                    .ToList();

                foreach (Sale sale in sales)
                {
                    var supplierWine = _context.SupplierWines.Find(sale.SupplierWineID);
                    wines.Add(_context.Wines.Find(supplierWine.WineID));

                    prices.Add(purchase.Price);
                }
            }

            return (wines, prices);
        }

        public void Update(Purchase purchase)
        {
            try
            {
                _context.Purchases.Update(purchase);
                _context.SaveChanges();
            }
            catch
            {
                throw new PurchaseException("Failed to update purchase.");
            }
        }

        public void Delete(Purchase purchase)
        {

            var foundPurchase = GetByID(purchase.ID);

            if (foundPurchase == null)
                throw new PurchaseException("Failed to get purchase by id.");

            try
            {
                _context.Purchases.Remove(foundPurchase);
                _context.SaveChanges();
            }
            catch
            {
                throw new PurchaseException("Failed to delete purchase.");
            }
        }
    }
}
