using WineSales.Domain.Models;

namespace WineSales.Domain.RepositoryInterfaces
{
    public interface IPurchaseRepository : ICrudRepository<Purchase>
    {
        List<Purchase> GetByPrice(double price);
        List<Purchase> GetByStatus(int status);
        (List<Wine>, List<double>) GetByCustomerID(int customerID);
        Purchase GetActive(int customerID, double price);
    }
}
