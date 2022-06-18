using WineSales.Domain.Models;

namespace WineSales.Domain.RepositoryInterfaces
{
    public interface IWineRepository : ICrudRepository<Wine>
    {
        List<Wine> GetByKind(string kind);
        List<Wine> GetByColor(string color);
        List<Wine> GetBySugar(string sugar);
        List<Wine> GetByVolume(double volume);
        List<Wine> GetByAlcohol(double alcohol);
        List<Wine> GetByAging(int aging);
        void Add(Wine wine, double purchasePrice, int percent);
        (List<Wine>, List<double>) GetCart(int customerID);
        (List<Wine>, List<double>) GetBySupplier(int supplierID);
        (List<Wine>, List<string>, List<double>) GetByAdmin();
        (List<Wine>, List<double>) GetRating();
    }
}
