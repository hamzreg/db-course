using WineSales.Domain.Models;

namespace WineSales.Domain.RepositoryInterfaces
{
    public interface ICheckRepository : ICrudRepository<Check>
    {
        List<Check> GetByPaymentMethod(string paymentMethod);
        List<Check> GetByShiftNumber(int shiftNumber);
        Check GetBySaleID(int saleID);
        (Check, Sale) GetByPurchase(int purchaseID);
    }
}
