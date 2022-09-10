using WineSales.Domain.Models;
using WineSales.Domain.RepositoryInterfaces;
using WineSales.Domain.Exceptions;

namespace WineSales.Data.Repositories
{
    public class SupplierWineRepository : ISupplierWineRepository
    {
        private readonly DataBaseContext _context;

        public SupplierWineRepository(DataBaseContext context)
        {
            _context = context;
        }

        public void Create(SupplierWine supplierWine)
        {
            try
            {
                _context.SupplierWines.Add(supplierWine);
                _context.SaveChanges();
            }
            catch
            {
                throw new SupplierWineException("Failed to create supplierWine.");
            }
        }

        public List<SupplierWine> GetAll()
        {
            return _context.SupplierWines.ToList();
        }

        public SupplierWine GetByID(int id)
        {
            return _context.SupplierWines.Find(id);
        }

        public List<SupplierWine> GetByWineID(int wineID)
        {
            return _context.SupplierWines.Where(supplierWine => supplierWine.WineID == wineID)
                .ToList();
        }

        public List<SupplierWine> GetByPrice(double price)
        {
            return _context.SupplierWines.Where(supplierWine => supplierWine.Price == price)
                .ToList();
        }

        public List<SupplierWine> GetByPercent(int percent)
        {
            return _context.SupplierWines.Where(supplierWine => supplierWine.Percent == percent)
                .ToList();
        }

        public (List<Wine>, List<double>, List<int>) GetBySupplierID(int supplierID)
        {
            var supplierWines = _context.SupplierWines
                .Where(supplierWine => supplierWine.SupplierID == supplierID)
                .ToList();

            var wines = new List<Wine>();
            var prices = new List<double>();
            var percentes = new List<int>();

            foreach (SupplierWine supplierWine in supplierWines)
            {
                wines.Add(_context.Wines.Find(supplierWine.WineID));
                prices.Add(supplierWine.Price);
                percentes.Add(supplierWine.Percent);
            }

            return (wines, prices, percentes);
        }

        public (List<int>, List<Wine>, List<double>) GetAllWine()
        {
            var supplierWines = GetAll();

            var ids = new List<int>();
            var wines = new List<Wine>();
            var prices = new List<double>();

            foreach (SupplierWine supplierWine in supplierWines)
            {
                ids.Add(supplierWine.ID);
                wines.Add(_context.Wines.Find(supplierWine.WineID));
                prices.Add(supplierWine.Price * (1 + supplierWine.Percent / 100.0));
            }

            return (ids, wines, prices);
        }

        public (List<Wine>, List<string>, List<double>) GetByAdmin()
        {
            var supplierWines = GetAll();

            var wines = new List<Wine>();
            var suppliers = new List<string>();
            var prices = new List<double>();

            foreach (SupplierWine supplierWine in supplierWines)
            {
                wines.Add(_context.Wines.Find(supplierWine.WineID));

                var supplier = _context.Suppliers.Find(supplierWine.SupplierID);
                suppliers.Add(supplier.Name);

                prices.Add(supplierWine.Price * (1 + supplierWine.Percent / 100.0));
            }

            return (wines, suppliers, prices);
        }

        public (List<Wine>, List<double>) GetRating()
        {
            var supplierWines = GetAll();
            supplierWines.Sort((a, b) => a.Rating.CompareTo(b.Rating));
            supplierWines.Reverse();

            var wines = new List<Wine>();
            var points = new List<double>();

            foreach (SupplierWine supplierWine in supplierWines)
            {
                wines.Add(_context.Wines.Find(supplierWine.WineID));
                points.Add(supplierWine.Rating);
            }

            return (wines, points);
        }

        public void Update(SupplierWine supplierWine)
        {
            try
            {
                _context.SupplierWines.Update(supplierWine);
                _context.SaveChanges();
            }
            catch
            {
                throw new SupplierWineException("Failed to update supplierWine.");
            }
        }

        public void Delete(SupplierWine supplierWine)
        {
            var foundSupplierWine = GetByID(supplierWine.ID);

            if (foundSupplierWine == null)
                throw new SupplierWineException("Failed to get supplierWine by id.");

            try
            {
                _context.SupplierWines.Remove(foundSupplierWine);
                _context.SaveChanges();
            }
            catch
            {
                throw new SupplierWineException("Failed to delete supplierWine.");
            }
        }
    }
}
