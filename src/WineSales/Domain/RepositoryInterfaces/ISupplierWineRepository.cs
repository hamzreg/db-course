using WineSales.Domain.Models;

namespace WineSales.Domain.RepositoryInterfaces
{
    public interface ISupplierWineRepository : ICrudRepository<SupplierWine>
    {
        List<SupplierWine> GetByWineID(int wineID);
        List<SupplierWine> GetByPrice(double price);
        List<SupplierWine> GetByPercent(int percent);
        (List<Wine>, List<double>) GetBySupplierID(int supplierID);
        (List<Wine>, List<double>) GetAllWine();
        (List<Wine>, List<double>) GetCart(int customerID);
        (List<Wine>, List<string>, List<double>) GetByAdmin();
        (List<Wine>, List<double>) GetRating();
    }
}
