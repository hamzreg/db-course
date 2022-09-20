using WineSales.Domain.Models;
using WineSales.Domain.RepositoryInterfaces;
using WineSales.Domain.Exceptions;

namespace WineSales.Data.Repositories
{
    public class CheckRepository : ICheckRepository
    {
        private readonly DataBaseContext _context;

        public CheckRepository(DataBaseContext context)
        {
            _context = context;
        }

        public void Create(Check check)
        {
            try
            {
                _context.Checks.Add(check);
                _context.SaveChanges();
            }
            catch
            {
                throw new CheckException("Failed to create check.");
            }
        }

        public List<Check> GetAll()
        {
            return _context.Checks.ToList();
        }

        public Check GetByID(int id)
        {
            return _context.Checks.Find(id);
        }

        public List<Check> GetByPaymentMethod(string paymentMethod)
        {
            return _context.Checks.Where(check => check.PaymentMethod == paymentMethod)
                .ToList();
        }

        public List<Check> GetByShiftNumber(int shiftNumber)
        {
            return _context.Checks.Where(check => check.ShiftNumber == shiftNumber)
                .ToList();
        }

        public Check GetBySaleID(int saleID)
        {
            return _context.Checks.FirstOrDefault(check => check.SaleID == saleID);
        }

/*        public (List<Check>, List<Sale>) GetByAdmin()
        {
            var checks = GetAll();
            var sales = new List<Sale>();

            foreach (Check check in checks)
            {
                sales.Add(_context.Sales.Find(check.SaleID));
            }

            return (checks, sales);
        }*/

        public (Check, Sale) GetByPurchase(int purchaseID)
        {
            var sale = _context.Sales.FirstOrDefault(sale => sale.PurchaseID == purchaseID);
            var check = GetBySaleID(sale.ID);

            return (check, sale);
        }

        public void Update(Check check)
        {
            try
            {
                _context.Checks.Update(check);
                _context.SaveChanges();
            }
            catch
            {
                throw new CheckException("Failed to update check.");
            }
        }

        public void Delete(Check check)
        {
            var foundCheck = GetByID(check.ID);

            if (foundCheck == null)
                throw new CheckException("Failed to get check by id.");

            try
            {
                _context.Checks.Remove(foundCheck);
                _context.SaveChanges();
            }
            catch
            {
                throw new CheckException("Failed to delete check.");
            }
        }
    }
}
