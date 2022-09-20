using WineSales.Domain.Models;

namespace WineSales.Domain.RepositoryInterfaces
{
    public interface ISaleRepository : ICrudRepository<Sale>
    {
        List<Sale> GetByPurchasePrice(double purchasePrice);
        List<Sale> GetBySellingPrice(double sellingPrice);
        List<Sale> GetByMargin(double margin);
        List<Sale> GetByCosts(double costs);
        List<Sale> GetByProfit(double profit);
        List<Sale> GetByWineNumber(int wineNumber);
        List<Sale> GetByDate(DateOnly date);
        Sale GetByPurchaseID(int purchaseID);
        (List<Wine>, List<DateOnly>, List<double>) GetBySupplierID(int supplierID);
        (List<Wine>, List<string>, List<Sale>) GetByAdmin();
    }
}
