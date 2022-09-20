using WineSales.Domain.Models;
using WineSales.Domain.RepositoryInterfaces;
using WineSales.Domain.Exceptions;

namespace WineSales.Data.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly DataBaseContext _context;

        public SaleRepository(DataBaseContext contex)
        {
            _context = contex;
        }

        public void Create(Sale sale)
        {
            try
            {
                _context.Sales.Add(sale);
                _context.SaveChanges();
            }
            catch
            {
                throw new SaleException("Failed to create sale.");
            }
        }

        public List<Sale> GetAll()
        {
            return _context.Sales.ToList();
        }

        public Sale GetByID(int id)
        {
            return _context.Sales.Find(id);
        }

        public List<Sale> GetByPurchasePrice(double purchasePrice)
        {
            return _context.Sales.Where(sale => sale.PurchasePrice == purchasePrice)
                .ToList();
        }

        public List<Sale> GetBySellingPrice(double sellingPrice)
        {
            return _context.Sales.Where(sale => sale.SellingPrice == sellingPrice)
                .ToList();
        }

        public List<Sale> GetByMargin(double margin)
        {
            return _context.Sales.Where(sale => sale.Margin == margin)
                .ToList();
        }

        public List<Sale> GetByCosts(double costs)
        {
            return _context.Sales.Where(sale => sale.Costs == costs)
                .ToList();
        }

        public List<Sale> GetByProfit(double profit)
        {
            return _context.Sales.Where(sale => sale.Profit == profit)
                .ToList();
        }

        public List<Sale> GetByWineNumber(int wineNumber)
        {
            return _context.Sales.Where(sale => sale.WineNumber == wineNumber)
                .ToList();
        }

        public List<Sale> GetByDate(DateOnly date)
        {
            return _context.Sales.Where(sale => sale.Date == date)
                .ToList();
        }

        public List<Sale> GetBySupplierWineID(int supplierWineID)
        {
            return _context.Sales.Where(sale => sale.SupplierWineID == supplierWineID)
                .ToList();
        }

        public Sale GetByPurchaseID(int purchaseID)
        {
            return _context.Sales.FirstOrDefault(sale => sale.PurchaseID == purchaseID);
        }

        public (List<Wine>, List<DateOnly>, List<double>) GetBySupplierID(int supplierID)
        {
            var supplierWines = _context.SupplierWines.Where(wine => wine.SupplierID == supplierID)
                .ToList();

            var wines = new List<Wine>();
            var dates = new List<DateOnly>();
            var prices = new List<double>();

            foreach (SupplierWine supplierWine in supplierWines)
            {
                var sales = GetBySupplierWineID(supplierWine.ID);

                foreach (Sale sale in sales)
                {
                    wines.Add(_context.Wines.Find(supplierWine.WineID));
                    dates.Add(sale.Date);
                    prices.Add(supplierWine.Price);
                }
            }

            return (wines, dates, prices);
        }

        public (List<Wine>, List<string>, List<Sale>) GetByAdmin()
        {
            var sales = GetAll();

            var wines = new List<Wine>();
            var suppliers = new List<string>();

            foreach (Sale sale in sales)
            {
                var supplierWine = _context.SupplierWines.Find(sale.SupplierWineID);
                wines.Add(_context.Wines.Find(supplierWine.WineID));

                var supplier = _context.Suppliers.Find(supplierWine.SupplierID);
                suppliers.Add(supplier.Name);
            }

            return (wines, suppliers, sales);
        }

        public void Update(Sale sale)
        {
            try
            {
                _context.Sales.Update(sale);
                _context.SaveChanges();
            }
            catch
            {
                throw new SaleException("Failed to update sale.");
            }
        }

        public void Delete(Sale sale)
        {
            var foundSale = GetByID(sale.ID);

            if (foundSale == null)
                throw new SaleException("Failed to get sale by id.");

            try
            {
                _context.Sales.Remove(foundSale);
                _context.SaveChanges();
            }
            catch
            {
                throw new SaleException("Failed to delete sale.");
            }
        }
    }
}
