using WineSales.Domain.Models;

namespace WineSales.Domain.RepositoryInterfaces
{
    public interface ISaleRepository : ICrudRepository<Sale>
    {
        List<int> GetPurchasePrices(List<int> wineID);
    }
}
