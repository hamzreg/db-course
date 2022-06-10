using WineSales.Domain.Models;

namespace WineSales.Domain.RepositoryInterfaces
{
    public interface IPurchaseRepository : ICrudRepository<Purchase>
    {
        List<Purchase> GetByPrice(double price);
    }
}
